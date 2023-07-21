import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { MoveRequest } from '../models/move-request.models';
import { environment } from 'src/environments/environment';
import { Observable } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class GameLogicService {

  constructor(private http: HttpClient) { }

  makeMove(player: number, state: number): Observable<number> {
    console.log(' Player: ' + player + " State: " + state);
    const moveRequest = new MoveRequest(player, state);
    return this.http.post<number>(`${environment.webApi}/Game/makeMove`, moveRequest);
  }
  
}
