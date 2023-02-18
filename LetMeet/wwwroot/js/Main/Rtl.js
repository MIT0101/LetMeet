
let isRtl=isRtlDirection();

//  rtl
document.getElementById("rtl1").addEventListener('click',(event)=>{

  const tableAndCardHeading= document.querySelectorAll(".card .table td , .table thead tr th , .card-header.card-header-warning");

  if (isRtl){
    document.querySelector("html").setAttribute("dir","");
    tableAndCardHeading.forEach(element => {
      element.style.textAlign = "left";
     });

  }else{
    document.querySelector("html").setAttribute("dir","rtl");

    tableAndCardHeading.forEach(element => {
    element.style.textAlign = "right";
   });
  }

   const notificationDropdownAndAccountDropdown= document.querySelectorAll(".dropdown-menu");

   notificationDropdownAndAccountDropdown.forEach(element => {
    element.classList.toggle("dropdown-menu-right");
    element.classList.toggle("dropdown-menu-left");
   });
  
   isRtl=!isRtl;  
  });

  function isRtlDirection(){
    return false;
  }
  
