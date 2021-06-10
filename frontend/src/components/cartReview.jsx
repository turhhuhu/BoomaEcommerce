import React, { Component } from "react";
import { connect } from "react-redux";

class CartReview extends Component {
  state = {};

  getCartItems = () => {
    return this.props.baskets?.map((basket) => {
      return basket.purchaseProducts
        .sort((a, b) => a.guid - b.guid)
        .map((purchaseProduct, index) => (
          <div className="col-6 row mr-2" key={index}>
            <div className="col">
              <strong>{purchaseProduct.product.name} </strong>
              <p>{basket?.store.storeName}</p>
            </div>
            <p className="col-8">
              {purchaseProduct.amount} x ${purchaseProduct.price} = Total: $
              {purchaseProduct.amount * purchaseProduct.price}{" "}
            </p>
          </div>
        ));
    });
  };

  render() {
    return (
      <div
        className="col-5 mx-auto card mb-3"
        style={{
          marginTop: "75px",
        }}
      >
        <article>
          <header className="mb-4 card-header">
            <h4>Review cart</h4>
          </header>
          <div className="row card-body">{this.getCartItems()}</div>
        </article>
        <article className="card-body border-top">
          <dl className="row">
            <dt className="col-sm-10">
              Subtotal:{" "}
              <span className="float-right text-muted">
                {this.props.baskets.reduce((basketTotalAmount, basket) => {
                  if (basket.purchaseProducts) {
                    return (
                      basketTotalAmount +
                      basket.purchaseProducts.reduce(
                        (totalAmount, purchaseProduct) =>
                          totalAmount + purchaseProduct.amount,
                        0
                      )
                    );
                  }
                  return 0;
                }, 0)}{" "}
                items
              </span>
            </dt>
            <dd className="col-sm-2 text-right">
              <strong>
                $
                {this.props.baskets.reduce((subTotalPrice, basket) => {
                  if (basket.purchaseProducts) {
                    return (
                      subTotalPrice +
                      basket.purchaseProducts.reduce(
                        (totalPrice, purchaseProduct) =>
                          totalPrice +
                          purchaseProduct.price * purchaseProduct.amount,
                        0
                      )
                    );
                  }
                  return 0;
                }, 0)}
              </strong>
            </dd>

            <dt className="col-sm-10">
              Discount:{" "}
              <span className="float-right text-muted">TBD% offer</span>
            </dt>
            <dd className="col-sm-2 text-danger text-right">
              <strong>$TBD</strong>
            </dd>

            <dt className="col-sm-10">Total:</dt>
            <dd className="col-sm-2 text-right">
              <strong className="h5 text-dark">$TBD</strong>
            </dd>
          </dl>
        </article>
        <div className="card-body border-top">
          <a href="/cart/payment" className="btn btn-primary float-md-right">
            {" "}
            Make Purchase <i className="fa fa-chevron-right"></i>{" "}
          </a>
          <a href="/home" className="btn btn-light">
            {" "}
            <i className="fa fa-chevron-left"></i> Continue shopping{" "}
          </a>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    baskets: store.user.cart.baskets,
  };
};

export default connect(mapStateToProps)(CartReview);
