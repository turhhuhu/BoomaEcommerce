import React, { Component } from "react";
import Header from "../components/header";
import CartReview from "../components/cartReview";
import { fetchUserCart } from "../actions/userActions";
import { connect } from "react-redux";
class CartReviewPage extends Component {
  state = {};
  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserCart());
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

export default connect()(CartReviewPage);
