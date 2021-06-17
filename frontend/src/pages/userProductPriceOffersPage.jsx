import React, { Component } from "react";
import { fetchProductOffers } from "../actions/userActions";
import Header from "../components/header";
import { Redirect } from "react-router";
import UserProductPriceOfferView from "../components/userProductPriceOffersView";
import { connect } from "react-redux";
import ProfileSideBar from "../components/profileSideBar";

class UserProductPriceOffersPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(fetchProductOffers());
    }
  }
  render() {
    if (!this.props.isAuthenticated) {
      return <Redirect to="/login" />;
    }
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1300px" }}>
          <section className="section-conten padding-y">
            <div className="row mt-2">
              <ProfileSideBar isProductOffers="true" />
              <UserProductPriceOfferView />
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
  };
};

export default connect(mapStateToProps)(UserProductPriceOffersPage);
