import React, { Component } from "react";
import { connect } from "react-redux";
import Dialog from "@material-ui/core/Dialog";
import DialogActions from "@material-ui/core/DialogActions";
import DialogContent from "@material-ui/core/DialogContent";
import DialogContentText from "@material-ui/core/DialogContentText";
import DialogTitle from "@material-ui/core/DialogTitle";
import { Alert } from "@material-ui/lab";
import { offerProductPrice } from "../actions/userActions";

class OfferPriceDialog extends Component {
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
          offerProductPrice({
            userGuid: this.props.userInfo?.guid,
            product: {
              guid: this.props.productGuid,
            },
            offerPrice: this.state.price,
          })
        )
        .then((success) => {
          if (success) {
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
              Offer
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
    error: store.user.error,
    userInfo: store.user.userInfo,
  };
};

export default connect(mapStateToProps)(OfferPriceDialog);
