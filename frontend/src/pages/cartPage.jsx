import React, { Component } from "react";
import CartView from "../components/cartView";
import Header from "../components/header";

class CartPage extends Component {
  state = {};
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <CartView />
        </section>
      </React.Fragment>
    );
  }
}

export default CartPage;
