import React, { Component } from "react";
import { connect } from "react-redux";
import AddStorePolicyDialog from "./addStorePolicyDialog";

class StorePolicyHeader extends Component {
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
          <h1 className="jumbotron-heading">{`${this.props.storeName} Policy`}</h1>
          <p>
            {this.props.myRole?.type === "ownership" ? (
              <button
                onClick={this.handleAddPolicy}
                className="btn btn-outline-primary my-2"
              >
                Add Policy
                <i className="ml-2 fa fa-plus"></i>
              </button>
            ) : null}
            <AddStorePolicyDialog
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
  };
};

export default connect(mapStateToProps)(StorePolicyHeader);
