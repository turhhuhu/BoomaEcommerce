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
      <div className="row mx-auto" style={{ maxWidth: "1200px" }}>
        <main className="col-md-9">
          {this.props.cart ? this.getCartItems() : null}
        </main>
        <aside className="col-md-3">
          <div className="card">
            <div className="card-body">
              <dl>
                <dt>Total price:</dt>
                <dd class="text-right">$69.97</dd>
              </dl>
              <dl>
                <dt>Discount:</dt>
                <dd class="text-right text-danger">- $10.00</dd>
              </dl>
              <dl>
                <dt>Total:</dt>
                <dd class="text-right text-dark b">
                  <strong>$59.97</strong>
                </dd>
              </dl>
              <hr />

              <a href="cart/payment" className="btn btn-primary btn-block">
                {" "}
                Make Purchase{" "}
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
