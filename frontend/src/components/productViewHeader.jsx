import React, { Component } from "react";

class ProductViewHeader extends Component {
  state = {};
  render() {
    return (
      <header className="border-bottom mb-4 pb-3">
        <div className="form-inline">
          <span className="mr-md-auto">
            {this.props.amount} Items were found{" "}
          </span>
          <select className="mr-2 form-control">
            <option>Latest items</option>
            <option>Trending</option>
            <option>Most Popular</option>
            <option>Cheapest</option>
          </select>
        </div>
      </header>
    );
  }
}

export default ProductViewHeader;
