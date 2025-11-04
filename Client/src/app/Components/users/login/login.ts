import { ChangeDetectionStrategy, Component, signal } from '@angular/core';
import { HttpUserService } from '../../../Services/HttpUserService';
import { UserDto } from '../../../Dtos/UserDto';
import { MatDialogRef, MatDialogActions, MatDialogContent, MatDialog} from '@angular/material/dialog';
import { MatButtonModule } from '@angular/material/button';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatIcon } from '@angular/material/icon';
import { MatInputModule } from '@angular/material/input';
import { FormsModule } from '@angular/forms';
import { Register } from '../register/register';

@Component({
  selector: 'app-login',
  imports: [FormsModule,MatIcon,MatInputModule,MatButtonModule,MatDialogActions,MatFormFieldModule,MatDialogContent],
  templateUrl: './login.html',
  styleUrl: './login.css',
  changeDetection: ChangeDetectionStrategy.OnPush,
})
export class Login {
  email = '';
  password = '';
  errorMessage = '';

  constructor (private readonly userService :HttpUserService,private readonly dialog: MatDialog, private readonly dialogRef: MatDialogRef<Login>) {}
  hide = signal(true);
    clickEvent(event: MouseEvent) {
    this.hide.set(!this.hide());
    event.stopPropagation();
  }
  Login() {
    const user: UserDto = {
      email: this.email,
      username: this.email,
      passwordHash: this.password
    };
    this.userService.login(user)
      .subscribe({
        next: () => {
          this.close(); // redirect after login
        },
        error: err => {
          console.error(err);
          this.errorMessage = 'Invalid Credentials';
        }
      });
  }
  Register() {
    const ref = this.dialog.open(Register, {
      width: '680px'
    });
    this.close();
  }
  close() {
    this.dialogRef.close();
  }
}
