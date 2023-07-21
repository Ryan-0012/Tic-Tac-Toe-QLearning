import { Component, EventEmitter, Input, Output } from '@angular/core';
import { MoveRequest } from 'src/app/models/move-request.models';
import { GameLogicService } from 'src/app/services/gameLogic.service';

const BoardSize: number = 3;
const EmptyCell: number = 0;
const PlayerX: number = 1;
const PlayerO: number = -1;

@Component({
  selector: 'app-tic-tac-toe',
  templateUrl: './tic-tac-toe.component.html',
  styleUrls: ['./tic-tac-toe.component.css']
})
export class TicTacToeComponent {
  activeColumns: number[] = Array(9).fill(0);
  count: number = 0;
  board: number[][] = [];

  pointsX: number = 0;
  pointsO: number = 0;
  @Output() pontosXChanged = new EventEmitter<number>();
  @Output() pontosOChanged = new EventEmitter<number>();
  
  playerH: number = 1;
  playerQ: number = -1;

  constructor(private game: GameLogicService) {
    
    this.initializeBoard();
  }

  private applyAction(position: number, player: number) {
    const row = Math.floor((position - 1) / BoardSize); // Fórmula para obter a linha
    const column = (position - 1) % BoardSize; // Fórmula para obter a coluna
    this.board[row][column] = player;
  }

  private initializeBoard() { 
    console.log("bbbbbbbbf");
    for (let i = 0; i < BoardSize; i++) {
      this.board[i] = [];
      for (let j = 0; j < BoardSize; j++) {
        this.board[i][j] = 0; // Inicializa todas as células como vazias
      }
    }
  }

  private getGameStateAsNumber(): number {
    let gameState: number = 0;
    let factor: number = 1;

    for (let i = 0; i < BoardSize; i++) {
      for (let j = 0; j < BoardSize; j++) {
        let cellValue: number = this.getCellValue(this.board[i][j]);
        gameState += cellValue * factor;
        factor *= 3;
      }
    }

    return gameState;
  }

  private getCellValue(cell: number): number {
    const EmptyCell: number = 0;
    const PlayerX: number = 1;
    const PlayerO: number = -1;

    if (cell === EmptyCell) {
      return 0; // Célula vazia
    } else if (cell === PlayerX) {
      return 1; // Jogador X
    } else if (cell === PlayerO) {
      return 2; // Jogador O
    } else {
      throw new Error("Valor de célula inválido: " + cell);
    }
  }

  public choosePosition(cell: number) {
    
    if (this.activeColumns[cell - 1] != 0)
      return;

    this.activeColumns[cell - 1] = this.playerH;
    this.getPlayerSymbol(cell);
    
    this.applyAction(cell, this.playerH);
    if(this.isGameOver()){
      console.log("aaaaaaaaal");
      this.initializeBoard();
      this.activeColumns = Array(9).fill(0);
      this.pontosXChanged.emit(this.pointsX);
      this.pontosOChanged.emit(this.pointsO);
      this.printBoard();
      return;
    }
    this.playsQ();
  }

  public playsQ() {
    const state = this.getGameStateAsNumber();
    
    this.game.makeMove(this.playerH, state).subscribe((cellQ) => {
      this.activeColumns[cellQ - 1] = this.playerQ;
      this.getPlayerSymbol(cellQ);
      this.applyAction(cellQ, this.playerQ);
      this.printBoard();
      if (this.isGameOver())
      {
        console.log("aaaaaaaaa");
        this.initializeBoard();
        this.activeColumns = Array(9).fill(0);
        this.pontosXChanged.emit(this.pointsX);
        this.pontosOChanged.emit(this.pointsO);
        this.printBoard();
      }
    })
    console.log("bbbbbbbb");
    
    
  }

  public isActive(cell: number): number {
    return this.activeColumns[cell - 1];
  }

  public getPlayerSymbol(cell: number): string {
    const symbol = this.isActive(cell);
    if (symbol === 1) {
      return 'X';
    } else if (symbol === -1) {
      return 'O';
    }
    return '';
  }

  private isGameOver(): boolean {
    return this.isWinner(PlayerX) || this.isWinner(PlayerO) || this.isDraw();
  }

  private isWinner(player: number): boolean {
    // Verificar linhas
    this.printBoard();
    console.log("playerl" + player);
    for (let i = 0; i < BoardSize; i++) {
      if (
        this.board[i][0] === player &&
        this.board[i][1] === player &&
        this.board[i][2] === player
      ) {
        console.log("Vencedor: Jogador " + (player === PlayerX ? "X" : "O"));
        
        player == PlayerX ? this.pointsX++ : this.pointsO++;
        return true;
      }
    }

    // Verificar colunas
    for (let j = 0; j < BoardSize; j++) {
      if (
        this.board[0][j] === player &&
        this.board[1][j] === player &&
        this.board[2][j] === player
      ) {
        console.log("Vencedor: Jogador " + (player === PlayerX ? "X" : "O"));
        player == PlayerX ? this.pointsX++ : this.pointsO++;
        return true;
      }
    }

    // Verificar diagonais
    if (
      this.board[0][0] === player &&
      this.board[1][1] === player &&
      this.board[2][2] === player
    ) {
      console.log("Vencedor: Jogador " + (player === PlayerX ? "X" : "O"));
      player == PlayerX ? this.pointsX++ : this.pointsO++;
      return true;
    }

    if (
      this.board[0][2] === player &&
      this.board[1][1] === player &&
      this.board[2][0] === player
    ) {
      console.log("Vencedor: Jogador " + (player === PlayerX ? "X" : "O"));
      player == PlayerX ? this.pointsX++ : this.pointsO++;
      return true;
    }

    return false;
  }

  private isDraw(): boolean {
    for (let i = 0; i < BoardSize; i++) {
      for (let j = 0; j < BoardSize; j++) {
        if (this.board[i][j] === EmptyCell) {
          return false; // Ainda há células vazias, o jogo não terminou em empate
        }
      }
    }

    console.log("Empate");
    return true; // Todas as células estão preenchidas, o jogo terminou em empate
  }

  public printBoard(): void {
    for (let i = 0; i < BoardSize; i++) {
      let row = "";
      for (let j = 0; j < BoardSize; j++) {
        if (this.board[i][j] === EmptyCell) {
          row += " ";
        } else if (this.board[i][j] === PlayerX) {
          row += "X";
        } else if (this.board[i][j] === PlayerO) {
          row += "O";
        }
        if (j < BoardSize - 1) {
          row += " | ";
        }
      }
      console.log(row);
      if (i < BoardSize - 1) {
        console.log("---------");
      }
    }
  }
}
