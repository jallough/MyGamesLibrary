import { Component, Inject } from '@angular/core';
import { GameGenre, GamesDto, GameStatus } from '../../../Dtos/GamesDto';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { HttpGameService } from '../../../Services/HttpGameService';
import { MAT_DIALOG_DATA, MatDialogModule, MatDialogRef } from '@angular/material/dialog';
import { HttpUserService } from '../../../Services/HttpUserService';
import { UserGameRelationDto } from '../../../Dtos/UserGameRelationDto';
import { MatFormField, MatLabel, MatOption, MatSelect } from '@angular/material/select';
import { CommonModule, DatePipe } from '@angular/common';
import { MatCardModule} from '@angular/material/card';
import { MatButtonModule } from '@angular/material/button';

@Component({
  selector: 'app-game-status-dialog',
  imports: [MatDialogModule,MatOption,MatSelect,MatLabel,MatFormField,DatePipe,ReactiveFormsModule,MatCardModule,MatButtonModule,CommonModule],
  templateUrl: './game-status-dialog.html',
  styleUrl: './game-status-dialog.css'
})
export class GameStatusDialog {
  form: any;
  mode: 'add' | 'edit' = 'add';
  // Build options arrays where value is numeric enum value and label is the name
  readonly GameGenre=GameGenre;
  statusOptions: Array<{ value: number; label: string }> = Object.keys(GameStatus)
    .filter(k => isNaN(Number(k)))
    .map(k => ({ value: (GameStatus as any)[k], label: k }));


  constructor(
    private readonly fb: FormBuilder,
    private readonly api: HttpGameService,
    private readonly dialogRef: MatDialogRef<GameStatusDialog>,
    private readonly userService: HttpUserService,
    @Inject(MAT_DIALOG_DATA) public data: { mode: 'add' | 'edit'; game?: UserGameRelationDto }
    ) {
    this.form = this.fb.group({
      status:['', Validators.required]
    });

    if (data?.mode === 'edit' && data.game) {
      this.mode = 'edit';
      this.form.patchValue({
        status: data.game.status
      });
    }
  }

  submit() {
    if (this.form.invalid) return;

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
    const finalPayload: UserGameRelationDto = {
      id : this.data?.game?.id,
      game: this.data.game?.game!,
      gameId:this.data.game?.game.id!,
      userId: this.userService.currentUserSig()!.id,
      status: this.form.value.status 
    };

    // Log the exact JSON string that will be sent so you can inspect it in Network tab
    console.debug('Final payload JSON sent to API:', JSON.stringify(finalPayload));
    
    if (this.mode === 'add') {
      this.api.AddGameRelation(finalPayload).subscribe({ next: res => this.dialogRef.close(true), error: handleError });
    } else if (this.mode === 'edit' && this.data.game) {
      this.api.UpdateGameRelation(finalPayload).subscribe({ next: res => this.dialogRef.close(true), error: handleError });
    }
  }

  close() {
    this.dialogRef.close(false);
  }
}
