import React, { Component } from "react";

class CartIcon extends Component {
  state = {};
  render() {
    return (
      <div className="widget-header  mr-3">
        <a href="/login" className="icon icon-sm rounded-circle border">
          <i className="fa fa-shopping-cart"></i>
        </a>
        <span className="badge badge-pill badge-danger notify">0</span>
      </div>
    );
  }
}

export default CartIcon;
