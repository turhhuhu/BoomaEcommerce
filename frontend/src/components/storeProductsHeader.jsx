import React, { Component } from "react";
import { connect } from "react-redux";
import AddStoreProductDialog from "./addStoreProductDialog";

class StoreProductsHeader extends Component {
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

  handleAddProduct = () => {
    this.setState({ isDialogOpen: true });
  };

  render() {
    return (
      <div className="container" style={{ maxWidth: "1400px" }}>
        <section className="text-center border-bottom">
          <h1 className="jumbotron-heading">{`${this.props.storeName} Products`}</h1>
          <p>
            {this.props.myRole?.type === "ownership" ||
            (this.props.myRole?.type === "management" &&
              this.props.myRole.permissions.canAddProduct) ? (
              <button
                onClick={this.handleAddProduct}
                className="btn btn-outline-primary my-2"
              >
                Add Product
                <i className="ml-2 fa fa-plus"></i>
              </button>
            ) : null}
            <AddStoreProductDialog
              guid={this.props.guid}
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
    error: store.store.error,
    myRole: store.user.storeRole,
  };
};

export default connect(mapStateToProps)(StoreProductsHeader);
