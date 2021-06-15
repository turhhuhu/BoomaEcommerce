import React, { Component } from "react";
import CounterOfferDialog from "./counterOfferDialog";

class StoreProductPriceOfferEntry extends Component {
  state = { isDialogOpen: false };
  handleCounterOffer = () => {
    this.setState({ isDialogOpen: true });
  };
  closeCounterOfferDialog = () => {
    this.setState({ isDialogOpen: false });
  };
  render() {
    return (
      <tr>
        <td>
          <span className="title mb-0">
            <strong>Offered price:</strong> $
            {this.props.productOffer.offerPrice}
          </span>
        </td>
        <td>
          <span className="title mb-0">
            <strong>Current Counter Offer price:</strong> $
            {this.props.productOffer.counterOfferPrice}
          </span>
        </td>
        <td>
          <span className="title mb-0">
            <strong>State:</strong> {this.props.productOffer.state}
          </span>
        </td>
        <td className="row  ">
          <div>
            <button className="btn btn-outline-primary col"> Approve </button>
          </div>
          <div>
            <button
              onClick={this.handleCounterOffer}
              className="btn btn-outline-info col ml-2"
            >
              {" "}
              Counter Offer
            </button>
            <CounterOfferDialog
              offerGuid={this.props.productOffer.guid}
              isDialogOpen={this.state.isDialogOpen}
              closeDialog={this.closeCounterOfferDialog}
            />
          </div>
          <div>
            <button className="btn btn-outline-secondary col ml-3">
              {" "}
              Decline{" "}
            </button>
          </div>
        </td>
      </tr>
    );
  }
}

export default StoreProductPriceOfferEntry;
