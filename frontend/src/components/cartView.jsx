import React, { Component } from "react";
import { connect } from "react-redux";
import CartItem from "./cartItem";
class CartView extends Component {
  state = { totalPrice: 0 };

  getCartItems = () => {
    return this.props.cart.baskets?.map((basket) => {
      return basket.purchaseProducts
        .sort((a, b) => a.guid - b.guid)
        .map((purchaseProduct, index) => (
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
      <div className="row mx-auto" style={{ maxWidth: "1000px" }}>
        <main className="col-md-9">
          {this.props.cart ? this.getCartItems() : null}
        </main>
        <aside className="col-md-3">
          <div className="card">
            <div className="card-body">
              <a href="cart/payment" className="btn btn-primary btn-block">
                {" "}
                Review Purchase{" "}
              </a>
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
