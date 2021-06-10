import React, { Component } from "react";
import { connect } from "react-redux";
import AddStoreDiscountDialog from "./addStoreDiscountDialog";

class StoreDiscountsHeader extends Component {
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

  handleAddDiscount = () => {
    this.setState({ isDialogOpen: true });
  };
  render() {
    return (
      <div className="container" style={{ maxWidth: "1400px" }}>
        <section className="text-center border-bottom">
          <h1 className="jumbotron-heading">{`${this.props.storeName} Discounts`}</h1>
          <p>
            {this.props.myRole?.type === "ownership" &&
            !this.props.storePolicy?.type ? (
              <button
                onClick={this.handleAddDiscount}
                className="btn btn-outline-primary my-2"
              >
                Add Discount
                <i className="ml-2 fa fa-plus"></i>
              </button>
            ) : null}
            <AddStoreDiscountDialog
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
    storeDiscount: store.store.storeDiscount,
  };
};

export default connect(mapStateToProps)(StoreDiscountsHeader);
