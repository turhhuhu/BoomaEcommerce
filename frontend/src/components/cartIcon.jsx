import React, { Component } from "react";
import { connect } from "react-redux";

class CartIcon extends Component {
  state = {};
  render() {
    return (
      <div className="widget-header  mr-3">
        <a href="/cart" className="icon icon-sm rounded-circle border">
          <i className="fa fa-shopping-cart"></i>
        </a>
        <span className="badge badge-pill badge-info notify">
          {this.props.baskets?.length > 0
            ? this.props.baskets.reduce((acc, basket) => {
                return acc + basket?.purchaseProducts?.length;
              }, 0)
            : 0}
        </span>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    baskets: store.user.cart.baskets,
  };
};

export default connect(mapStateToProps)(CartIcon);
