import React, { Component } from "react";
import { connect } from "react-redux";
import Header from "../components/header";
import { Redirect } from "react-router";
import PurchaseHistoryView from "../components/purchaseHistoryView";
import ProfileSideBar from "../components/profileSideBar";
import { fetchUserPurchaseHistory } from "../actions/userActions";

class UserPurchaseHistoryPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchUserPurchaseHistory());
    }
  }

  render() {
    if (!this.props.isAuthenticated) {
      return <Redirect to="/login" />;
    }
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <div className="ml-5 row">
              <ProfileSideBar isPurchaseHistory="true" />
              <PurchaseHistoryView
                hrefPrefix={`/user/purchases`}
                purchases={this.props.purchaseHistory}
              />
            </div>
          </section>
        </div>
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

export default connect(mapStateToProps)(UserPurchaseHistoryPage);
