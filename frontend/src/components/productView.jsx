import React, { Component } from "react";
import { connect } from "react-redux";
import Product from "./product";
import ProductViewHeader from "./productViewHeader";

class ProductView extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <ProductViewHeader />
        <main class="col-md-12.5">
          <div class="row">
            <Product />
            <Product />
            <Product />
            <Product />
            <Product />
            <Product />
          </div>
        </main>
      </React.Fragment>
    );
  }
}

export default connect()(ProductView);
