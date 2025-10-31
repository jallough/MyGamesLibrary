import { GamesDto, GameStatus } from "./GamesDto";

export class UserGameRelationDto{
    id?:number;
    userId?:number;
    game!:GamesDto;
    gameId!:number;
    status?:GameStatus;
}