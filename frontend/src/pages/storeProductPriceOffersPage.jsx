import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchStoreProductOffers } from "../actions/storeActions";
import { fetchUserStoreRole } from "../actions/userActions";
import Header from "../components/header";
import StoreProductPriceOffersView from "../components/storeProductPriceOffersView";
import StoreSideBar from "../components/storeSideBar";

class StoreProductPriceOffersPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props
        .dispatch(fetchUserStoreRole(this.props.match.params.guid))
        .then((action) => {
          if (action) {
            if (action.payload.response.type === "ownership")
              this.props.dispatch(
                fetchStoreProductOffers(action.payload.response.guid)
              );
          }
        });
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1300px" }}>
          <section className="section-conten padding-y">
            <div className="row mt-2">
              <StoreSideBar
                isProductOffers="true"
                guid={this.props.match.params.guid}
                colClass="col-2"
              />
              <StoreProductPriceOffersView />
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

export default connect(mapStateToProps)(StoreProductPriceOffersPage);
