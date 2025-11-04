import { Component, signal, ChangeDetectorRef } from '@angular/core';
import { HttpUserService } from '../../../Services/HttpUserService';
import { FormBuilder, FormGroup, Validators, FormsModule, ReactiveFormsModule } from '@angular/forms';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { MatDialog, MatDialogActions, MatDialogContent, MatDialogRef } from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { Login } from '../login/login';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-register',
  imports: [CommonModule, FormsModule, ReactiveFormsModule, MatIcon, MatInputModule, MatButtonModule, MatDialogActions, MatFormFieldModule, MatDialogContent],
  templateUrl: './register.html',
  styleUrl: './register.css'
})
export class Register {
  registerForm: FormGroup;
  errorMessage = '';

  constructor(
    private readonly userService: HttpUserService,
    private readonly dialog: MatDialog,
    private readonly dialogRef: MatDialogRef<Register>,
    private fb: FormBuilder,
    private cdr: ChangeDetectorRef
  ) {
    this.registerForm = this.fb.group({
      username: ['', [Validators.required, Validators.minLength(3)]],
      email: ['', [Validators.required, Validators.email]],
      password: ['', [
        Validators.required, 
        Validators.minLength(6),
        Validators.pattern('^(?=.*[A-Za-z])(?=.*\\d)[A-Za-z\\d]{6,}$')
      ]],
      confirmPassword: ['', [Validators.required]]
    }, {
      validator: this.passwordMatchValidator
    });
  }

  private passwordMatchValidator(g: FormGroup) {
    const password = g.get('password')?.value;
    const confirmPassword = g.get('confirmPassword')?.value;
    return password === confirmPassword ? null : { 'mismatch': true };
  }

  getErrorMessage(controlName: string): string {
    const control = this.registerForm.get(controlName);
    if (control?.errors) {
      if (control.errors['required']) return `${controlName.charAt(0).toUpperCase() + controlName.slice(1)} is required`;
      if (control.errors['email']) return 'Please enter a valid email address';
      if (control.errors['minlength']) return `${controlName.charAt(0).toUpperCase() + controlName.slice(1)} must be at least ${control.errors['minlength'].requiredLength} characters`;
      if (control.errors['pattern'] && controlName === 'password') return 'Password must contain at least one letter and one number';
    }
    if (controlName === 'confirmPassword' && this.registerForm.errors?.['mismatch']) {
      return 'Passwords do not match';
    }
    return '';
  }
  hide = signal(true);
    clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }
  
  Register() {    
    if (this.registerForm.invalid) {
      this.registerForm.markAllAsTouched();
      
      // Check each control and set specific error messages
      for (const key of Object.keys(this.registerForm.controls)) {
        const control = this.registerForm.get(key);
        if (control?.errors) {
          this.errorMessage = this.getErrorMessage(key);
          this.cdr.detectChanges();
          break;
        }
      }

      // Check for password match error specifically
      if (this.registerForm.errors?.['mismatch']) {
        this.errorMessage = 'Passwords do not match';
        this.cdr.detectChanges();
      }
      return;
    }

  // Clear any previous error messages
  this.errorMessage = '';
  this.cdr.detectChanges();
    const formValue = this.registerForm.value;
    const user = {
      email: formValue.email,
      username: formValue.username,
      passwordHash: formValue.password
    };

    this.userService.register(user)
      .subscribe({
        next: () => {
          this.dialog.open(Login, { width: '680px'});
          this.close();
        },
        error: err => {
          console.error(err);
          console.error(err.error);
          this.errorMessage = err.error;
          this.cdr.detectChanges();
        }
      });
  }

  close() {
    this.dialogRef.close();
  }
}
