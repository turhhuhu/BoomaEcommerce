import React, { Component } from "react";
import { connect } from "react-redux";
import { createPurchase, fetchUserCart } from "../actions/userActions";
import { turnCartIntoPurchase } from "../utils/utilFunctions";

class PurchaseReview extends Component {
  state = {};

  getCartItems = () => {
    return this.props.baskets?.map((basket) => {
      return basket.purchaseProducts
        .sort((a, b) => a.guid - b.guid)
        .map((purchaseProduct) => (
          <tr key={purchaseProduct.guid}>
            <td>
              <strong className="title mb-0">
                {purchaseProduct?.product.name}{" "}
              </strong>
              <br />
              <var className="price text-muted">$TBD</var>
            </td>
            <td>
              <div className="float-right">
                {" "}
                Store: <br /> {basket?.store.storeName}{" "}
              </div>
            </td>
          </tr>
        ));
    });
  };

  makePurchase = () => {
    const purchaseobj = {
      purchase: turnCartIntoPurchase(
        this.props.cart,
        this.props.userInfo?.guid
      ),
      paymentDetails: this.props.paymentInfo,
      supplyDetails: this.props.deliveryInfo,
    };
    console.log(purchaseobj);
    this.props
      .dispatch(
        createPurchase({
          purchase: turnCartIntoPurchase(
            this.props.cart,
            this.props.userInfo?.guid
          ),
          paymentDetails: this.props.paymentInfo,
          supplyDetails: this.props.deliveryInfo,
        })
      )
      .then((success) => {
        if (success) {
          this.props.dispatch(fetchUserCart());
        }
      });
  };

  render() {
    return (
      <div
        className="row col-5 mx-auto card"
        style={{
          marginTop: "75px",
        }}
      >
        <header className="card-header">
          <strong className="d-inline-block mr-3">Purchase Review</strong>
          <button
            onClick={this.makePurchase}
            className="btn btn-primary float-right"
          >
            {" "}
            Confirm purhcase{" "}
          </button>
        </header>
        <div className="card-body">
          <div className="row">
            <div className="col-md-8">
              <h6 className="text-muted">Delivery to</h6>
              <p style={{ maxWidth: "350px" }}>
                Full Name: {this.props?.deliveryInfo.name} <br />
                Address: {this.props?.deliveryInfo?.address}{" "}
              </p>
            </div>
            <div className="col-md-4">
              <h6 className="text-muted">Payment</h6>
              <span>
                ****{" "}
                {this.props?.paymentInfo.cardNumber.substring(
                  this.props?.paymentInfo.cardNumber.length - 4,
                  this.props?.paymentInfo.cardNumber.length
                )}{" "}
                <i className="fa fa-lg fa-cc-visa"></i>
                <i className="fa fa-lg fa-cc-mastercard"></i>
                <i className="fa fa-lg fa-cc-amex"></i>
              </span>
              <p>
                <span className="b">Total: $TBD </span>
              </p>
            </div>
          </div>
        </div>
        <div className="table-responsive">
          <table className="table table-hover">
            <tbody>{this.getCartItems()}</tbody>
          </table>
        </div>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    baskets: store.user.cart.baskets,
    deliveryInfo: store.user.deliveryInfo,
    paymentInfo: store.user.paymentInfo,
    cart: store.user.cart,
    userInfo: store.user.userInfo,
  };
};
export default connect(mapStateToProps)(PurchaseReview);
