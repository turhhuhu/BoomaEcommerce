import React, { Component } from "react";

class ProductViewHeader extends Component {
  state = {};
  render() {
    return (
      <header class="border-bottom mb-4 pb-3">
        <div class="form-inline">
          <span class="mr-md-auto">32 Items found </span>
          <select class="mr-2 form-control">
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
