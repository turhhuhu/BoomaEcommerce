import React, { Component } from "react";
import { connect } from "react-redux";
import {
  addStoreDicountRootPolicy,
  addStoreDicountSubPolicy,
  fetchStoreDiscountPolicy,
} from "../actions/storeActions";
import AddStorePolicyDialog from "./addStorePolicyDialog";

class StoreDiscountPolicyHeader extends Component {
  state = {
    isDialogOpen: false,
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  closeDialog = () => {
    this.setState({ isDialogOpen: false });
  };

  handleAddPolicy = () => {
    this.setState({ isDialogOpen: true });
  };
  render() {
    return (
      <div className="container" style={{ maxWidth: "1400px" }}>
        <section className="text-center border-bottom">
          <h1 className="jumbotron-heading">{`${this.props.storeName} Discount Policy`}</h1>
          <p>
            {this.props.myRole?.type === "ownership" &&
            !this.props.storeDiscountPolicy?.type ? (
              <button
                onClick={this.handleAddPolicy}
                className="btn btn-outline-primary my-2"
              >
                Add Policy
                <i className="ml-2 fa fa-plus"></i>
              </button>
            ) : null}
            <AddStorePolicyDialog
              addRootPolicy={addStoreDicountRootPolicy(this.props.discountGuid)}
              addSubPolicy={addStoreDicountSubPolicy(this.props.discountGuid)}
              fetchPolicy={fetchStoreDiscountPolicy(this.props.discountGuid)}
              isRoot="true"
              storeGuid={this.props.storeGuid}
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeDialog}
            />
          </p>
        </section>
      </div>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    myRole: store.user.storeRole,
    storeDiscountPolicy: store.store.storeDiscountPolicy,
  };
};

export default connect(mapStateToProps)(StoreDiscountPolicyHeader);
