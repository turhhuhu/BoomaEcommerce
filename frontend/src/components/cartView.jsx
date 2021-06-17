import React, { Component } from "react";
import { connect } from "react-redux";
import CartItem from "./cartItem";
class CartView extends Component {
  state = { totalPrice: 0 };

  getCartItems = () => {
    return this.props.cart.baskets?.map((basket) => {
      return basket.purchaseProducts
        ?.sort((a, b) => a.guid - b.guid)
        .map((purchaseProduct, index) => {
          console.log(purchaseProduct);
          return (
            <CartItem
              key={index}
              product={purchaseProduct.product}
              purchaseProductGuid={purchaseProduct.guid}
              quantity={purchaseProduct.amount}
              price={purchaseProduct.price}
              maxQuantity={purchaseProduct.product?.amount}
              storeName={basket.store?.storeName}
              basketGuid={basket.guid}
            />
          );
        });
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
              {this.props.cart?.baskets.length > 0 ? (
                <a
                  href={
                    this.props.isAuthenticated ? "cart/review" : "cart/guest"
                  }
                  className="btn btn-primary btn-block"
                >
                  {this.props.isAuthenticated
                    ? " Review Purchase "
                    : " Continue to purchase "}
                </a>
              ) : null}

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
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(CartView);
