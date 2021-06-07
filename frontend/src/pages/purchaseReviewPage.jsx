import React, { Component } from "react";
import Header from "../components/header";
import { connect } from "react-redux";
import PurchaseReview from "../components/purchaseReview";
class PurchaseReviewPage extends Component {
  state = {};
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

export default connect()(PurchaseReviewPage);
