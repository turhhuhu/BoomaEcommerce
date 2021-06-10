import React, { Component } from "react";
import { connect } from "react-redux";
import { fetchUserStoreRole } from "../actions/userActions";
import {
  addStoreDicountRootPolicy,
  addStoreDicountSubPolicy,
  fetchAllStoreProducts,
  fetchStoreDiscountPolicy,
  removeStoreDiscountPolicy,
} from "../actions/storeActions";
import Header from "../components/header";
import PolicyTree from "../components/policyTree";
import StoreDiscountPolicyHeader from "../components/storeDiscountPolicyHeader";

class StoreDiscountPolicyPage extends Component {
  state = {};

  componentDidMount() {
    if (this.props.match.params.guid) {
      this.props.dispatch(fetchUserStoreRole(this.props.match.params.guid));
      this.props.dispatch(
        fetchStoreDiscountPolicy(this.props.match.params.discountGuid)(
          this.props.match.params.guid
        )
      );
      this.props.dispatch(fetchAllStoreProducts(this.props.match.params.guid));
    }
  }
  render() {
    return (
      <React.Fragment>
        <Header />
        <div className="container" style={{ maxWidth: "1000px" }}>
          <section className="section-conten padding-y">
            <StoreDiscountPolicyHeader
              storeName={this.props.storeInfo?.storeName}
              storeGuid={this.props.match.params.guid}
              discountGuid={this.props.match.params.discountGuid}
            />
            <div className="row mt-2">
              <PolicyTree
                policies={this.props.storeDiscountPolicy}
                addRootPolicy={addStoreDicountRootPolicy(
                  this.props.match.params.discountGuid
                )}
                addSubPolicy={addStoreDicountSubPolicy(
                  this.props.match.params.discountGuid
                )}
                fetchPolicy={fetchStoreDiscountPolicy(
                  this.props.match.params.discountGuid
                )}
                removePolicy={removeStoreDiscountPolicy(
                  this.props.match.params.discountGuid
                )}
                storeGuid={this.props.match.params.guid}
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
    storeInfo: store.store.storeInfo,
    storeDiscountPolicy: store.store.storeDiscountPolicy,
  };
};

export default connect(mapStateToProps)(StoreDiscountPolicyPage);
