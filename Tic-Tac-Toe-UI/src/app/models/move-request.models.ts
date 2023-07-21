export class MoveRequest {
    player: number;
    state: number;
  
    constructor(player: number, state: number) {
      this.player = player;
      this.state = state;
    }
}
  