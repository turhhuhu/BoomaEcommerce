import React, { Component } from "react";

class MessageIcon extends Component {
  state = {};
  render() {
    return (
      <div className="widget-header  mr-3">
        <a href="/login" className="icon icon-sm rounded-circle border">
          <i className="fa fa-envelope"></i>
        </a>
        <span className="badge badge-pill badge-info notify">0</span>
      </div>
    );
  }
}

export default MessageIcon;
