import React, { Component } from "react";
import { connect } from "react-redux";
import CartItem from "./cartItem";
class CartView extends Component {
  state = {};

  handlePurchase = () => {
    console.log(this.props.cart);
  };

  getCartItems = () => {
    return this.props.cart.baskets?.map((basket) => {
      return basket.purchaseProducts.map((purchaseProduct, index) => (
        <CartItem
          key={index}
          product={purchaseProduct.product}
          purchaseProductGuid={purchaseProduct.guid}
          price={purchaseProduct.price}
          maxQuantity={purchaseProduct.amount}
          storeName={basket.store?.storeName}
          basketGuid={basket.guid}
        />
      ));
    });
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
