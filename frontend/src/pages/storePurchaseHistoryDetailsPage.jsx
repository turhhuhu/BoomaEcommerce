import React, { Component } from "react";
import { connect } from "react-redux";
import { Redirect } from "react-router";
import Header from "../components/header";
import StorePurchaseHistoryDetails from "../components/StorePurchaseHistoryDetails";

class StorePurchaseHistoryDetailsPage extends Component {
  state = {};

  findStorePurchase = () => {
    return this.props.purchaseHistory?.find(
      (storePurchase) => storePurchase.guid === this.props.match.params.guid
    );
  };
  render() {
    if (!this.props.isAuthenticated) {
      return <Redirect to="/login" />;
    }
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <StorePurchaseHistoryDetails
            storePurchase={this.findStorePurchase()}
          />
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    purchaseHistory: store.store.purchaseHistory,
  };
};

export default connect(mapStateToProps)(StorePurchaseHistoryDetailsPage);
