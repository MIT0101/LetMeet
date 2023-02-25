let isDark=isCurrentThemeDark();
  // event for theme
  document.getElementById("themeBtn").addEventListener('click',themSwitcher);

  function themSwitcher(event){
    document.querySelector("body").classList.toggle("dark-version");
    if(isDark){
        document.querySelector(".sidebar").setAttribute("data-background-color","white");
    }else{
        document.querySelector(".sidebar").setAttribute("data-background-color","black");

    }
   const centerNavItems= document.querySelectorAll(".navbar .collapse .navbar-nav .nav-item .nav-link:not(.btn) .material-icons");

   centerNavItems.forEach(element => {
    element.classList.toggle("md-dark");
   });

      const checkboxsAndNotifications = document.querySelectorAll(".form-check .form-check-sign .check , .dropdown-menu , .dropdown-item");
   checkboxsAndNotifications.forEach(element => {
       element.classList.toggle("md-dark");

   });

      //switch modals

      let modals = document.querySelectorAll(".modal-content");

      modals.forEach(element => {
          element.classList.toggle("dark-version");

      });

      let closeBtns = document.querySelectorAll(".close");

      closeBtns.forEach(element => {
          element.classList.toggle("dark-version");

      });

 
   //switch Mod
   isDark=!isDark;
  }

function isCurrentThemeDark(){
  return false;
}
