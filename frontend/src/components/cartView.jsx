import React, { Component } from "react";
import { connect } from "react-redux";
import CartItem from "./cartItem";
class CartView extends Component {
  state = {};

  handlePurchase = () => {
    console.log(this.props.cart);
  };

  getCartItems = () => {
    let store = undefined;
    let basketGuid = undefined;
    return this.props.cart.baskets
      ?.flatMap((basket) => {
        store = basket.store;
        basketGuid = basket.guid;
        return basket.purchaseProducts;
      })
      .map((purchaseProduct) => (
        <CartItem
          key={purchaseProduct.guid}
          product={purchaseProduct.product}
          purchaseProductGuid={purchaseProduct.guid}
          price={purchaseProduct.price}
          maxQuantity={purchaseProduct.amount}
          storeName={store?.storeName}
          basketGuid={basketGuid}
        />
      ));
  };

  render() {
    return (
      <div className="row mx-auto" style={{ maxWidth: "1200px" }}>
        <main className="col-md-9">
          {this.props.cart ? this.getCartItems() : null}
        </main>
        <aside className="col-md-3">
          <div className="card">
            <div className="card-body">
              <dt>Total price:</dt>
              <dd className="text-right text-dark b">
                {this.state.totalPrice}
              </dd>
              <hr />

              <button
                onClick={this.handlePurchase}
                className="btn btn-primary btn-block"
              >
                {" "}
                Make Purchase{" "}
              </button>
              <a
                href="/products"
                className="btn btn-outline-secondary btn-block"
              >
                Continue Shopping
              </a>
            </div>
          </div>
        </aside>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    cart: store.user.cart,
  };
};

export default connect(mapStateToProps)(CartView);
