import React, { Component } from "react";
import { connect } from "react-redux";
import {
  approveProductOffer,
  fetchStoreProductOffers,
} from "../actions/storeActions";
import CounterOfferDialog from "./counterOfferDialog";

class StoreProductPriceOfferEntry extends Component {
  state = { isDialogOpen: false };
  handleCounterOffer = () => {
    this.setState({ isDialogOpen: true });
  };
  closeCounterOfferDialog = () => {
    this.setState({ isDialogOpen: false });
  };
  handleApproveOffer = () => {
    this.props
      .dispatch(
        approveProductOffer(
          this.props.storeRole.guid,
          this.props.productOffer.guid
        )
      )
      .then((action) => {
        if (action) {
          this.props.dispatch(
            fetchStoreProductOffers(this.props.storeRole.guid)
          );
        }
      });
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
          {this.props.productOffer.counterOfferPrice ? (
            <span className="title mb-0">
              <div>
                <strong>Current Counter Offer price:</strong> ${" "}
                {this.props.productOffer.counterOfferPrice}
              </div>
            </span>
          ) : null}
        </td>
        <td>
          <span className="title mb-0">
            <strong>State:</strong> {this.props.productOffer.state}
          </span>
        </td>
        <td className="row  ">
          <div>
            <button
              onClick={this.handleApproveOffer}
              className="btn btn-outline-primary col"
            >
              {" "}
              Approve{" "}
            </button>
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

const mapStateToProps = (store) => {
  return {
    storeRole: store.user.storeRole,
  };
};

export default connect(mapStateToProps)(StoreProductPriceOfferEntry);
