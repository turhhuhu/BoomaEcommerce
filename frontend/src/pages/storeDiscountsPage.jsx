import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserStoreRole } from "../actions/userActions";
import {
  fetchAllStoreProducts,
  fetchStoreDiscounts,
} from "../actions/storeActions";
import Header from "../components/header";
import StoreDiscountsHeader from "../components/storeDiscountsHeader";
import DiscountsTree from "../components/discountsTree";
import StoreSideBar from "../components/storeSideBar";

class StoreDiscountsPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.match.params.guid) {
      this.props.dispatch(fetchUserStoreRole(this.props.match.params.guid));
      this.props.dispatch(fetchStoreDiscounts(this.props.match.params.guid));
      this.props.dispatch(fetchAllStoreProducts(this.props.match.params.guid));
    }
  }
  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <StoreDiscountsHeader
              storeName={this.props.storeInfo?.storeName}
              storeGuid={this.props.match.params.guid}
            />
            <div className="row mt-2">
              <StoreSideBar
                isDiscounts="true"
                guid={this.props.match.params.guid}
                colClass="col-3"
              />
              <DiscountsTree storeGuid={this.props.match.params.guid} />
            </div>
          </section>
        </div>
      </React.Fragment>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    storeInfo: store.store.storeInfo,
  };
};

export default connect(mapStateToProps)(StoreDiscountsPage);
