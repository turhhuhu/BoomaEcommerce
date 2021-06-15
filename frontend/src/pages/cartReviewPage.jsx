import React, { Component } from "react";
import Header from "../components/header";
import CartReview from "../components/cartReview";
import {
  fetchCartDiscountedPrice,
  fetchUserCart,
} from "../actions/userActions";
import { connect } from "react-redux";
import {
  turnCartIntoPurchase,
  turnCartIntoPurchaseAsGuest,
} from "../utils/utilFunctions";
class CartReviewPage extends Component {
  state = {};
  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserCart()).then(() =>
        this.props.dispatch(
          fetchCartDiscountedPrice({
            purchase: turnCartIntoPurchase(
              this.props.cart,
              this.props.userInfo?.guid
            ),
          })
        )
      );
    } else {
      this.props.dispatch(
        fetchCartDiscountedPrice({
          purchase: turnCartIntoPurchaseAsGuest(
            this.props.cart,
            this.props.guestInformation
          ),
        })
      );
    }
  }
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <CartReview />
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    cart: store.user.cart,
    userInfo: store.user.userInfo,
    guestInformation: store.user.guestInformation,
  };
};

export default connect(mapStateToProps)(CartReviewPage);
