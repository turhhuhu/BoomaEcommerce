import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchStorePurchaseHistory } from "../actions/storeActions";
import Header from "../components/header";
import PurchaseHistoryView from "../components/purchaseHistoryView";
import StoreSideBar from "../components/storeSideBar";

class StorePurchaseHistoryPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.isAuthenticated) {
      this.props.dispatch(
        fetchStorePurchaseHistory(this.props.match.params.guid)
      );
    }
  }

  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <div className="row mt-2">
              <StoreSideBar
                isPurchaseHistory="true"
                guid={this.props.match.params.guid}
                colClass="col-3"
              />
              <PurchaseHistoryView
                hrefPrefix={`/store/${this.props.match.params.guid}/purchases`}
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
    purchaseHistory: store.store.purchaseHistory,
  };
};

export default connect(mapStateToProps)(StorePurchaseHistoryPage);
