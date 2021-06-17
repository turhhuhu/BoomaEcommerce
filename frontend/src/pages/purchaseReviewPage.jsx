import React, { Component } from "react";
import Header from "../components/header";
import { connect } from "react-redux";
import PurchaseReview from "../components/purchaseReview";
import { fetchUserInfo } from "../actions/userActions";
class PurchaseReviewPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserInfo());
    }
  }
  render() {
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <PurchaseReview />
        </section>
      </React.Fragment>
    );
  }
}
const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
  };
};

export default connect(mapStateToProps)(PurchaseReviewPage);
