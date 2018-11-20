using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace BangazonWorkforce.Models.ViewModels
{

    /*
        * Author: Ricky Bruner
        * Purpose: A very simplostic view model that accepts a List of Training Programs. This Data is used on the Index view page for training programs.
    */
    public class TrainingProgramIndexViewModel
    {

        public List<TrainingProgram> TrainingPrograms { get; set; }

    }
}
