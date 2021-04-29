import React, { Component } from "react";

class NotificationIcon extends Component {
  state = {};
  render() {
    return (
      <div className="widget-header  mr-3">
        <a href="/login" className="icon icon-sm rounded-circle border">
          <i className="fa fa-bell white"></i>
        </a>
        <span className="badge badge-pill badge-secondary notify">0</span>
      </div>
    );
  }
}

export default NotificationIcon;
