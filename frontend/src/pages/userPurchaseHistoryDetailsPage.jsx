import React, { Component } from "react";
import { connect } from "react-redux";
import { Redirect } from "react-router";
import Header from "../components/header";
import UserPurchaseHistoryDetails from "../components/userPurchaseHistoryDetails";
class UserPurchaseHistoryDetailsPage extends Component {
  state = {};

  findPurchase = () => {
    return this.props.purchaseHistory?.find(
      (purchase) => purchase.guid === this.props.match.params.guid
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
          <UserPurchaseHistoryDetails purchase={this.findPurchase()} />
        </section>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    isAuthenticated: store.auth.isAuthenticated,
    purchaseHistory: store.user.purchaseHistory,
  };
};

export default connect(mapStateToProps)(UserPurchaseHistoryDetailsPage);
