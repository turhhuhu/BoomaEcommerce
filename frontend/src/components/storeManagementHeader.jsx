import React, { Component } from "react";
import { connect } from "react-redux";
import AddStoreManagerDialog from "./addStoreManagerDialog";
import AddStoreOwnerDialog from "./addStoreOwnerDialog";

class StoreManagementHeader extends Component {
  state = {
    isAddOwnerDialogOpen: false,
    isAddManagerDialogOpen: false,
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  closeAddOwnerDialog = () => {
    this.setState({ isAddOwnerDialogOpen: false });
  };

  closeAddManagerDialog = () => {
    this.setState({ isAddManagerDialogOpen: false });
  };

  handleAddOwner = () => {
    this.setState({ isAddOwnerDialogOpen: true });
  };

  handleAddManager = () => {
    this.setState({ isAddManagerDialogOpen: true });
  };

  render() {
    return (
      <div className="container" style={{ maxWidth: "1400px" }}>
        <section className="text-center border-bottom">
          <h1 className="jumbotron-heading">{`${this.props.storeName} Managment`}</h1>
          <p>
            <button
              onClick={this.handleAddOwner}
              className="btn btn-outline-primary my-2"
            >
              Add owner
              <i className="ml-2 fa fa-plus"></i>
            </button>
            <AddStoreOwnerDialog
              guid={this.props.guid}
              isDialogOpen={this.state.isAddOwnerDialogOpen}
              closeDialog={this.closeAddOwnerDialog}
            />
            <button
              onClick={this.handleAddManager}
              className="ml-3 btn btn-outline-primary my-2"
            >
              Add manager
              <i className="ml-2 fa fa-plus"></i>
            </button>
            <AddStoreManagerDialog
              guid={this.props.guid}
              isDialogOpen={this.state.isAddManagerDialogOpen}
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

export default connect(mapStateToProps)(StoreManagementHeader);
