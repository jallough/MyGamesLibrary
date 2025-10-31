import { Component, Inject, Injectable } from '@angular/core';
import { CommonModule } from '@angular/common';
import { MAT_DIALOG_DATA,
  MatDialogModule,
  MatDialogRef } from '@angular/material/dialog';
import { ReactiveFormsModule, FormBuilder, Validators } from '@angular/forms';
import { MatFormFieldModule } from '@angular/material/form-field';
import { MatInputModule } from '@angular/material/input';
import { MatButtonModule } from '@angular/material/button';
import { MatAutocompleteModule } from '@angular/material/autocomplete';
import { MatSelectModule } from '@angular/material/select';
import { MatDatepickerModule } from '@angular/material/datepicker';
import { MatNativeDateModule } from '@angular/material/core';
import { HttpGameService } from '../../../Services/HttpGameService';
import { GamesDto, GameGenre, GameStatus } from '../../../Dtos/GamesDto';
import {provideNativeDateAdapter} from '@angular/material/core';

@Component({
  selector: 'app-game-dialog',
  standalone: true,
  providers:[ provideNativeDateAdapter() ],
  imports: [CommonModule, ReactiveFormsModule, MatFormFieldModule, MatInputModule, MatButtonModule, MatAutocompleteModule, MatDatepickerModule, MatNativeDateModule, 
    MatSelectModule, MatDialogModule],
  templateUrl: './game-dialog.html',
  styleUrl: './game-dialog.css'
})
export class GameDialog {
  form: any;

  mode: 'add' | 'edit' = 'add';
  // Build options arrays where value is numeric enum value and label is the name
  genreOptions: Array<{ value: number; label: string }> = Object.keys(GameGenre)
    .filter(k => isNaN(Number(k)))
    .map(k => ({ value: (GameGenre as any)[k], label: k }));

  statusOptions: Array<{ value: number; label: string }> = Object.keys(GameStatus)
    .filter(k => isNaN(Number(k)))
    .map(k => ({ value: (GameStatus as any)[k], label: k }));


  constructor(
    private readonly fb: FormBuilder,
    private readonly api: HttpGameService,
    private readonly dialogRef: MatDialogRef<GameDialog>,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'add' | 'edit'; game?: GamesDto }
  ) {
    this.form = this.fb.group({
      title: ['', Validators.required],
      genre: [null],
      releaseDate: [''],
      imageUrl: ['']
    });

    if (data?.mode === 'edit' && data.game) {
      this.mode = 'edit';
      // Convert numeric enum values to their names for the form fields
      const rd = data.game.releaseDate ? new Date(data.game.releaseDate) : '';
      this.form.patchValue({
        title: data.game.title,
        genre: data.game.genre,
        releaseDate: rd,
        imageUrl: data.game.imageUrl,
        status: data.game.status
      });
    }
  }

  submit() {
    if (this.form.invalid) return;
    const payload = this.form.value as Partial<GamesDto>;
    // Normalize releaseDate: if Date -> ISO date string (yyyy-mm-dd), if string keep as-is
    const rd = this.form.value.releaseDate;
    if (rd instanceof Date && !isNaN(rd.getTime())) {
      // send full ISO datetime so server can parse into DateTime
      payload.releaseDate = rd.toISOString();
    } else if (typeof rd === 'string') {
      // try to parse string to ISO; fallback to raw string if parsing fails
      const parsed = new Date(rd);
      payload.releaseDate = isNaN(parsed.getTime()) ? rd : parsed.toISOString();
    }

    const handleError = (err: any) => {
      console.error('API error', err);
      const apiErr = err?.error;
      if (apiErr?.errors) {
        console.error('Validation errors:', apiErr.errors);
        alert('Validation error: ' + JSON.stringify(apiErr.errors));
      } else if (apiErr?.title) {
        alert(apiErr.title);
      } else {
        alert('An error occurred while saving the game. See console for details.');
      }
    };

    // Build strict payload matching server's GamesEntity shape.
    // Use PascalCase keys to match the server model exactly (Name, Genre, ReleaseDate, ImageUrl, Status)
    const finalPayload: any = {
      Id : this.data.game?.id ?? 0,
      Title: payload.title ?? '',
      Genre: payload.genre ,
      ReleaseDate: payload.releaseDate?.split('T')[0] ?? '',
      ImageUrl: payload.imageUrl ?? ''
    };

    // Log the exact JSON string that will be sent so you can inspect it in Network tab
    console.debug('Final payload JSON sent to API:', JSON.stringify(finalPayload));

    if (this.mode === 'add') {
      this.api.create(finalPayload).subscribe({ next: res => this.dialogRef.close(true), error: handleError });
    } else if (this.mode === 'edit' && this.data.game) {
      this.api.update(this.data.game.id, finalPayload).subscribe({ next: res => this.dialogRef.close(true), error: handleError });
    }
  }

  close() {
    this.dialogRef.close(false);
  }
}
