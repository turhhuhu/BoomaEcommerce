import React, { Component } from "react";
import { connect } from "react-redux";

class StoreManagementHeader extends Component {
  state = {
    isAddOwnerDialogOpen: false,
    isAddmanagerDialogOpen: false,
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  closeAddOwnerDialog = () => {
    this.setState({ isAddOwnerDialogOpen: false });
  };

  closeAddOwnerDialog = () => {
    this.setState({ isAddmanagerDialogOpen: false });
  };

  handleAddOwner = () => {
    this.setState({ isAddOwnerDialogOpen: true });
  };

  handleAddManager = () => {
    this.setState({ isAddmanagerDialogOpen: true });
  };

  render() {
    return (
      <div className="container" style={{ maxWidth: "1400px" }}>
        <section className="text-center border-bottom">
          <h1 className="jumbotron-heading">{`${this.props.storeName} Products`}</h1>
          <p>
            <button
              onClick={this.handleAddProduct}
              className="btn btn-outline-primary my-2"
            >
              Add owner
              <i className="ml-2 fa fa-plus"></i>
            </button>
            <AddStoreOwnerDialog
              guid={this.props.guid}
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeAddOwnerDialog}
            />
          </p>
          <p>
            <button
              onClick={this.handleAddProduct}
              className="btn btn-outline-primary my-2"
            >
              Add manager
              <i className="ml-2 fa fa-plus"></i>
            </button>
            <AddStoreManagerDialog
              guid={this.props.guid}
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeAddManagerDialog}
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
  };
};

export default connect(mapStateToProps)(StoreProductsHeader);
