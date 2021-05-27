import React, { Component } from "react";
import { connect } from "react-redux";
import AddStoreDialog from "./addStoreDialog";

class UserStoresHeader extends Component {
  state = { isDialogOpen: false };
  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  closeDialog = () => {
    this.setState({ isDialogOpen: false });
  };

  handleAddStore = () => {
    this.setState({ isDialogOpen: true });
  };
  render() {
    return (
      <div className="container" style={{ maxWidth: "1400px" }}>
        <section className="text-center border-bottom">
          <h1 className="jumbotron-heading">{`${this.props.username} Stores`}</h1>
          <p>
            <button
              onClick={this.handleAddStore}
              className="btn btn-outline-primary my-2"
            >
              Add Store
              <i className="ml-2 fa fa-plus"></i>
            </button>
            <AddStoreDialog
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeDialog}
            />
          </p>
        </section>
      </div>
    );
  }
}

export default connect()(UserStoresHeader);
