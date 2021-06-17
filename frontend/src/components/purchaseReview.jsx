import { Alert } from "@material-ui/lab";
import React, { Component } from "react";
import { connect } from "react-redux";
import { Redirect } from "react-router";
import {
  clearGuestCart,
  createPurchase,
  fetchUserCart,
} from "../actions/userActions";
import {
  turnCartIntoPurchase,
  turnCartIntoPurchaseAsGuest,
} from "../utils/utilFunctions";

class PurchaseReview extends Component {
  state = { success: false, loading: false };

  startLoading = () => {
    setTimeout(() => {
      this.setState({ loading: false });
    }, 3000);
  };

  getCartItems = () => {
    return this.props.baskets?.map((basket) => {
      return basket.purchaseProducts
        .sort((a, b) => a.guid - b.guid)
        .map((purchaseProduct, index) => (
          <tr key={index}>
            <td>
              <strong className="title mb-0">
                {purchaseProduct?.product.name}{" "}
              </strong>
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
    this.setState({ loading: true });
    this.startLoading();
    if (this.props.isAuthenticated) {
      this.props
        .dispatch(
          createPurchase({
            purchase: turnCartIntoPurchase(
              this.props.cart,
              this.props.userInfo?.guid
            ),
            paymentDetails: this.props.paymentInfo,
            supplyDetails:
              Object.keys(this.props.deliveryInfo).length === 0
                ? undefined
                : this.props.deliveryInfo,
          })
        )
        .then((success) => {
          if (success) {
            this.props.dispatch(fetchUserCart());
            this.setState({ success: true });
          }
        });
    } else {
      this.props
        .dispatch(
          createPurchase({
            purchase: turnCartIntoPurchaseAsGuest(
              this.props.cart,
              this.props.guestInformation
            ),
            paymentDetails: this.props.paymentInfo,
            supplyDetails:
              Object.keys(this.props.deliveryInfo).length === 0
                ? undefined
                : this.props.deliveryInfo,
          })
        )
        .then((success) => {
          if (success) {
            this.setState({ loading: true });
            this.startLoading();
            this.props.dispatch(clearGuestCart());
            this.setState({ success: true });
          }
        });
    }
  };

  render() {
    if (!this.state.loading && this.state.success) {
      return <Redirect to="/home" />;
    }
    return (
      <div
        className="row col-5 mx-auto card"
        style={{
          marginTop: "75px",
        }}
      >
        <header className="card-header">
          <strong className="d-inline-block mr-3">Purchase Review</strong>
          {this.props.isFetching ? (
            <div className="d-flex justify-content-center">
              <div className="spinner-border text-primary" role="status"></div>
            </div>
          ) : (
            <button
              onClick={this.makePurchase}
              className="btn btn-primary float-right"
            >
              {" "}
              Confirm purhcase{" "}
            </button>
          )}
        </header>
        {(this.props.error || this.state.error) && (
          <Alert severity="error" onClick={() => this.setState(null)}>
            {this.props?.error || this.state?.error}
          </Alert>
        )}
        {this.state.success && (
          <div>
            <Alert severity="success" onClick={() => this.setState(null)}>
              Successfuly purchased cart!
              <br /> redirecting to home page...{" "}
            </Alert>
          </div>
        )}
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
                {this.props?.paymentInfo.cardNumber?.substring(
                  this.props?.paymentInfo.cardNumber?.length - 4,
                  this.props?.paymentInfo.cardNumber?.length
                )}{" "}
                <i className="fa fa-lg fa-cc-visa"></i>
                <i className="fa fa-lg fa-cc-mastercard"></i>
                <i className="fa fa-lg fa-cc-amex"></i>
              </span>
              <p>
                <span className="b">
                  Total: ${this.props.cart.discountedPrice}{" "}
                </span>
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
    isFetching: store.user.isFetching,
    error: store.user.error,
    isAuthenticated: store.auth.isAuthenticated,
    guestInformation: store.user.guestInformation,
  };
};
export default connect(mapStateToProps)(PurchaseReview);
