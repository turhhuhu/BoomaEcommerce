import React, { Component } from "react";
import { connect } from "react-redux";
import { Redirect } from "react-router";
class PurchaseHistoryDetails extends Component {
  state = {};
  render() {
    if (!this.props.isAuthenticated) {
      return <Redirect to="/login" />;
    }
    return (
      <React.Fragment>
        <Header />
        <section className="section-conten padding-y">
          <PurchaseHistoryDetails
            purchase={this.props.purchaseHistory?.find(
              (purchase) => purchase.guid === this.props.match.params.guid
            )}
          />
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

export default connect(mapStateToProps)(PurchaseHistoryDetails);
