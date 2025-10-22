export class GamesDto {
    id!: number;
    title!: string;
    genre!: GameGenre;
    releaseDate!: string;
    imageUrl!: string;
    status?: GameStatus;
}

export enum GameStatus {
    Playing,
    Completed, 
    OnHold ,
    Dropped ,
    PlanToPlay,
}

export enum GameGenre {
    Action ,
    Adventure ,
    RPG ,
    Strategy ,
    Simulation ,
    Sports ,
    Puzzle ,
    Horror ,
    MMO ,
    Indie  
}