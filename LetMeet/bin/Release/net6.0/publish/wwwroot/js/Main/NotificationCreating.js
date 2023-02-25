const MeShowNotification = (title, message, icon,
   type, delay,postionA,postionB) => {
  if (!icon) {
    icon = "add_alert";
  }
  if (!type) {
    type = "success";
  }
  $.notify(
    { title, message, icon }
    , { delay, type ,placement: { from: postionA, align: postionB } });
};

const showNotificationm = (e, a) => {
  type = ["", "info", "danger", "success", "warning", "rose", "primary"],
    color = Math.floor(6 * Math.random() + 1),
    $.notify({ icon: "add_alert", message: "Welcome to <b>Material Dashboard Pro</b> - a beautiful admin panel for every web developer." }, { type: type[color], timer: 3e3, placement: { from: e, align: a } })
}