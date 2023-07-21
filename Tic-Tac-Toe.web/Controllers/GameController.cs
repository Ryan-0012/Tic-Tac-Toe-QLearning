using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Reflection.Metadata;
using Tic_Tac_Toe.Game;
using Tic_Tac_Toe.Models;
using Tic_Tac_Toe.Services;

namespace Tic_Tac_Toe.web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class GameController : ControllerBase
    {
        private readonly GameLogic _game;
        private readonly QLearning2 _qLearning2;
        public GameController(GameLogic gameLogic, QLearning2 qLearning2)
        {
            _game = gameLogic;
            _qLearning2 = qLearning2;
        }

        [HttpPost("makeMove")]
        public int MakeMove([FromBody] MoveRequest moveRequest)
        {
            int player = moveRequest.Player;
            int state = moveRequest.State;
            // Fazer a jogada no jogo da velha
            int positionQLearning = _game.MakeMove(player, state);

            // Retornar o estado atual do jogo
            return positionQLearning;
        }

        [HttpPost("train")]
        public void Train([FromBody] int numEp)
        {
            _qLearning2.Train(numEp, 0.1m);
        }

        [HttpPost("play")]
        public void Play()
        {
            _qLearning2.PlayAgainstHuman();
        }
    }
}
