import { Component, Input } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent {
  pointsX: number = 0;
  pointsO: number = 0;

  @Input() pointX: number = 0;  
  constructor(){
    
  }

  receberDadosDoFilhoX(pontosX: number) {
    this.pointsX = pontosX; // Atualizar a propriedade pointsX com o valor emitido pelo TicTacToeComponent
  }

  receberDadosDoFilhoO(pontosO: number) {
    this.pointsO = pontosO; // Atualizar a propriedade pointsX com o valor emitido pelo TicTacToeComponent
  }
}
