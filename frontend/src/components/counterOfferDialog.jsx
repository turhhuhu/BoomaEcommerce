import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { Alert } from "@material-ui/lab";
import {
  counterOfferProductPrice,
  fetchStoreProductOffers,
} from "../actions/storeActions";

class CounterOfferDialog extends Component {
  state = {
    error: undefined,
    price: "",
  };

  handleChange = (event) => {
    this.setState({
      [event.target.name]: event.target.value,
    });
  };

  handleClose = (event) => {
    event.preventDefault();
    this.props.closeDialog();
  };

  handleSubmit = (event) => {
    if (!this.state.price) {
      this.setState({ error: "Please fill offered price" });
      return;
    } else {
      event.preventDefault();
      this.setState({ error: undefined });
      this.props
        .dispatch(
          counterOfferProductPrice(
            this.props.storeRole.guid,
            this.props.offerGuid,
            this.state.price
          )
        )
        .then((success) => {
          if (success) {
            this.props.dispatch(
              fetchStoreProductOffers(this.props.storeRole.guid)
            );
            this.props.closeDialog();
          }
        });
    }
  };
  render() {
    return (
      <Dialog
        open={this.props.isDialogOpen}
        onClose={this.handleClose}
        aria-labelledby="form-dialog-title"
      >
        <DialogTitle id="form-dialog-title">Offer new price</DialogTitle>
        <form>
          <DialogContent>
            <DialogContentText>
              Please fill out the new offered price:
            </DialogContentText>
            <label>Price:</label>
            <input
              type="text"
              className="form-control mb-2"
              name="price"
              required
              value={this.state.price}
              onChange={this.handleChange}
            ></input>
          </DialogContent>
          <DialogActions>
            <button
              className="btn btn-outline-primary my-2"
              onClick={this.handleClose}
            >
              Cancel
            </button>
            <button
              className="btn btn-outline-primary my-2"
              onClick={this.handleSubmit}
            >
              Counter Offer
            </button>
          </DialogActions>
          {(this.props.error || this.state.error) && (
            <Alert severity="error" onClick={() => this.setState(null)}>
              {this.props.error || this.state.error}
            </Alert>
          )}
        </form>
      </Dialog>
    );
  }
}

const mapStateToProps = (store) => {
  return {
    error: store.store.error,
    storeRole: store.user.storeRole,
  };
};

export default connect(mapStateToProps)(CounterOfferDialog);
