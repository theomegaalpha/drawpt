using DrawPT.Common.Models.Game;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DrawPT.Common.Interfaces
{
    public interface IAssessmentService
    {
        public Task<PlayerAnswer> AssessAnswerAsync(string originalPrompt, PlayerAnswer answer);
    }
}
