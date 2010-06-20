using Orchard.Mvc.ViewModels;
using Orchard.Pages.Models;

namespace Orchard.Pages.ViewModels {

	// this is more a form dto than a view model, this is used in a view model though (as well as a command handling parameter)
	public class PageCreateViewModel 
		
		//: BaseViewModel  // I'm questioning the inheritance here, is it used for the command handling? seems to work without it
	
	{

        public ContentItemViewModel<Page> Page { get; set; }
        public bool PromoteToHomePage { get; set; }
				public PageCommand Command { get; set; }
    }
}