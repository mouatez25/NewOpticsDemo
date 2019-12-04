using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NewOptics.SVC;
using System.ServiceModel;

namespace NewOptics.Administrateur
{
    [CallbackBehavior(UseSynchronizationContext = false)]
    public  class ICallback : IServiceCliniqueCallback
    {
        #region Delegates

        public delegate void CallbackEventHandler(object source, CallbackEvent e);
        public delegate void CallbackEventHandler1(object source, CallbackEventJoin e);
        public delegate void CallbackEventHandler2(object source, CallbackEventUserLeave e);
        public delegate void CallbackEventHandler3(object source, CallbackEventMessageRecu e);
        public delegate void CallbackEventHandler4(object source, CallbackEventWriting e);

        public delegate void CallbackEventHandler5(object source, CallbackEventInsertMembershipOptic e);
        public delegate void CallbackEventHandler6(object source, CallbackEventInsertF1 e);

        public delegate void CallbackEventHandler7(object source, CallbackEventInsertClientV e);
        public delegate void CallbackEventHandler8(object source, CallbackEventInsertFacture e);
        public delegate void CallbackEventHandler9(object source, CallbackEventInsertTcab e);
        public delegate void CallbackEventHandler10(object source, CallbackEventInsertListProdf e);
        public delegate void CallbackEventHandler11(object source, CallbackEventInsertListfacturevente e);
        public delegate void CallbackEventHandler12(object source, CallbackEventInsertVerreAssocie e);
        public delegate void CallbackEventHandler13(object source, CallbackEventInsertSupplement e);
        public delegate void CallbackEventHandler14(object source, CallbackEventInsertIncompatibilite e);
        public delegate void CallbackEventHandler15(object source, CallbackEventInsertListMonture e);
        public delegate void CallbackEventHandler16(object source, CallbackEventInsertParam e);
        public delegate void CallbackEventHandler17(object source, CallbackEventInsertListLentilleClient e);
        public delegate void CallbackEventHandler18(object source, CallbackEventInsertDepense e);
        public delegate void CallbackEventHandler19(object source, CallbackEventInsertMotifDepense e);
        public delegate void CallbackEventHandler20(object source, CallbackEventInsertFourn e);
        public delegate void CallbackEventHandler21(object source, CallbackEventInsertProdf e);
        public delegate void CallbackEventHandler22(object source, CallbackEventInsertProduit e);
        public delegate void CallbackEventHandler23(object source, CallbackEventInsertRecouf e);
        public delegate void CallbackEventHandler24(object source, CallbackEventInsertRecept e);
        public delegate void CallbackEventHandler25(object source, CallbackEventInsertdepaief e);
        public delegate void CallbackEventHandler26(object source, CallbackEventInsertam e);
        public delegate void CallbackEventHandler27(object source, CallbackEventInsertF1List e);
        public delegate void CallbackEventHandler29(object source, CallbackEventInsertLentille e);
        public delegate void CallbackEventHandler30(object source, CallbackEventInsertTarifVerre e);
        public delegate void CallbackEventHandler31(object source, CallbackEventInsertVerre e);
        public delegate void CallbackEventHandler32(object source, CallbackEventInsertDepeiment e);
        public delegate void CallbackEventHandler33(object source, CallbackEventInsertCommande e);
        public delegate void CallbackEventHandler34(object source, CallbackEventInsertMonture e);
        public delegate void CallbackEventHandler35(object source, CallbackEventInsertLentilleClient e);
        public delegate void CallbackEventHandler36(object source, CallbackEventInsertExamenOptométrique e);
        public delegate void CallbackEventHandler37(object source, CallbackEventInsertExamenBinoculaire e);
        public delegate void CallbackEventHandler38(object source, CallbackEventInsertAdaptationLentille e);
        public delegate void CallbackEventHandler39(object source, CallbackEventInsertAnamnese e);
        public delegate void CallbackEventHandler40(object source, CallbackEventInsertRendezVou e);
        public delegate void CallbackEventHandler41(object source, CallbackEventInsertAppointment e);
        public delegate void CallbackEventHandler42(object source, CallbackEventInsertResource e);
        public delegate void CallbackEventHandler43(object source, CallbackEventInsertPaint e);
        public delegate void CallbackEventHandler47(object source, CallbackEventReceiveFile e);
        public delegate void CallbackEventHandler48(object source, CallbackEventReceiveWhisper e);
        public delegate void CallbackEventHandler49(object source, CallbackEventInsertDepeiementMultiple e);
        public delegate void CallbackEventHandler50(object source, CallbackEventInsertProdfRecept e);
        public delegate void CallbackEventHandler51(object source, CallbackEventInsertListImage e);
        public delegate void CallbackEventHandler52(object source, CallbackEventInsertStatu e);
        public delegate void CallbackEventHandler53(object source, CallbackEventInsertMotifTable e);
        public delegate void CallbackEventHandler54(object source, CallbackEventInsertDepeiementMultipleFournisseur e);
        public delegate void CallbackEventHandler55(object source, CallbackEventInsertMontureSupplement e);
        public delegate void CallbackEventHandler56(object source, CallbackEventInsertCatSupp e);
        public delegate void CallbackEventHandler57(object source, CallbackEventInsertMarque e);

        public delegate void CallbackEventHandler58(object source, CallbackEventInsertFamilleProduit e);
        //   public List<SVC.Client> OnlineClients = new List<SVC.Client>();


        //   public abstract void RefreshClients(List<Client> clients);
        public void RefreshClients(List<Client> clients)
        {
            fireCallbackEvent(clients);

        }
        public void UserJoin(Client client)
        {
            UserJoined(client);
        }
        public void UserLeave(Client client)
        {
            UserLeaved(client);
        }
        public void Receive(Message msg)
        {
            ReceivedMessage(msg);
        }
        public void IsWritingCallback(Client client)
        {
            UserWrite(client);
        }
        public void RefreshMembership(List<MembershipOptic> MembershipOptic)
        {
            CallbackInsertMembershipOptic(MembershipOptic);
        }
      
        public void RefreshClientV(SVC.ClientV medecin,int oper)
        {
            CallbackInsertClientV(medecin,oper);

        }
        public void RefreshF1(F1 medecin, int oper)
        {
            CallbackInsertF1(medecin, oper);
        }
        public void RefreshParametre(SVC.Param medecin)
        {
            CallbackParam(medecin);
        }
    
        public void RefreshMotifDepense(List<MotifDepense> medecin)
        {
            CallbackRefreshMotifDepense(medecin);
        }
        public void RefreshDepense(Depense medecin,int oper)
        {
            CallbackRefreshDepense(medecin,oper);
        }
        public void RefreshFourn(List<Fourn> medecin)
        {
             CallbackRefreshFourn(medecin);
        }
        public void RefreshProduit(Produit medecin, int oper)
        {
            CallbackRefreshProduit(medecin, oper);
        }
      
        public void RefreshFamilleProduit(List<FamilleProduit> medecin)
        {
            CallbackRefreshFamilleProduit(medecin);
        }
        public void RefreshProdf(Prodf medecin,int oper)
        {
            CallbackRefreshProdf(medecin,oper);
        }
        public void RefreshRecouf(Recouf medecin,int oper)
        {
            CallbackRefreshRecouf(medecin,oper);
        }
        public void RefreshRecept(List<Recept> medecin)
        {
            CallbackRefreshRecept(medecin);
        }
        public void Refreshdepaief( depaief  medecin,int code)
        {
            CallbackRefreshdepaief(medecin,code);
        }
        public void Refresham(List<am> medecin)
        {
            CallbackRefresham(medecin);
        }
      
       
        public void RefreshDepeiment(Depeiment medecin,int oper)
        {
            CallbackRefreshDepeiment(medecin,oper);
        }
      
        
        public void ReceiverFile(FileMessage fileMsg, Client receiver)
        {
            CallbackRefreshReceiveFile(fileMsg,receiver);
        }
        public void ReceiveWhisper(Message msg, Client receiver)
        {
            CallbackRefreshReceiveWhisper(msg, receiver);

        }
        public void RefreshDepeimentMultiple(List<DepeiementMultiple> medecin)
        {
            CallbackRefreshDepeiementMultiple(medecin);
        }

        public void RefreshFacture(Facture medecin, int operf)
        {
            CallbackRefreshFacture(medecin,operf);
        }
        public void RefreshTCab(List<Tcab> medeicn)
        {
            CallbackRefreshTcab(medeicn);
        }
        public void RefreshProdflist(List<Prodf> medecin, int operF)
        {
            CallbackRefreshListProdf(medecin,operF);
        }
        public void RefreshFactureListe(List<Facture> medecin)
        {
            CallbackRefreshListfacturevente(medecin);
        }
        public void RefreshMarque(List<Marque> medecin)
        {
            CallbackRefreshMarque(medecin);
        }

        public void RefreshTarifVerre(List<TarifVerre> medeicn)
        {
            CallbackRefreshTarifVerre(medeicn);
        }
        public void RefreshVerre(Verre medeicn,int oper)
        {
            CallbackRefreshVerre(medeicn,oper);
        }
        public void RefreshLentille(Lentille medeicn,int oper)
        {
            CallbackRefreshLentille(medeicn,oper);
        }
        public void RefreshCommande(SVC.Commande medecin, int oper)
        {
            CallbackRefreshCommande(medecin, oper);

        }
        public void RefreshMonture(Monture medeicn, int operF)
        {
            CallbackRefreshMonture(medeicn, operF);
        }
            public void RefreshLentilleClient(LentilleClient medeicn, int operF)
        {
            CallbackRefreshLentilleClient(medeicn, operF);
        }
        public void RefreshDicom(List<DicomFichier> medecin)
        {
            CallbackRefreshDicomClient(medecin);
        }
        public void RefreshExamensOptométriques(examenopto medeicn, int operF)
        {
            CallbackRefreshExamenOptométrique(medeicn, operF);
        }
        public void RefreshExamenBinoculaire(ExamenBinoculaire medeicn, int operF)
        {
            CallbackRefreshExamenBinoculaire(medeicn, operF);
        }
        public void RefreshAnamnese(Anamnese medeicn, int operF)
        {
            CallbackRefreshAnamnese(medeicn, operF);
        }
        public void RefreshAdaptationLentille(AdaptationLentille medeicn, int operF)
        {
            CallbackRefreshAdaptationLentille(medeicn, operF);
        }
        public void RefreshRendezVou(RendezVou medeicn, int operF)
        {
            CallbackRefreshRendezVou(medeicn, operF);
        }
        public void RefreshAppointment(Appointment medeicn, int operF)
        {
            CallbackRefreshAppointment(medeicn, operF);
        }
        public void RefreshResource(Resource medeicn, int operF)
        {
            CallbackRefreshResource(medeicn, operF);
        }
        public void RefreshPaint(Paint medeicn, int operF)
        {
            CallbackRefreshPaint(medeicn, operF);
        }
        public void RefreshMotifTable(List<MotifTable> medecin)
        {
            CallbackRefreshMotifTable(medecin);
        }
        public void RefreshStatu(List<Statu> medecin)
        {
            CallbackRefreshStatu(medecin);
        }
        public void RefreshDepeiementMultipleFournisseur(List<DepeiementMultipleFournisseur> medecin)
        {
            CallbackRefreshDepeiementMultipleFournisseur(medecin);
        }
        public void RefreshIncompatibilite(Incompatibilite medeicn, int operF)
        {
            CallbackRefreshIncompatibilite(medeicn, operF);
        }
        public void RefreshVerreAssocie(VerreAssocie medeicn, int operF)
        {
            CallbackRefreshVerreAssocie(medeicn, operF);
        }
        public void RefreshSupplement(Supplement medeicn, int operF)
        {
            CallbackRefreshSupplement(medeicn, operF);
        }
        public void RefreshCatSupp(List<CatSupp> medecin)
        {
            CallbackRefreshCatSupp(medecin);
            
        }
        public void RefreshMontureSupplement(MontureSupplement medeicn, int operF)
        {
            CallbackRefreshMontureSupplement(medeicn, operF);
        }
        public void RefreshMontureList(List<Monture> medeicn)
        {
            CallbackRefreshMontureList(medeicn);
        }
        public void RefreshLentilleClientList(List<LentilleClient> medeicn)
        {
            CallbackRefreshLentilleClientList(medeicn);
        }
        public void RefreshF1ListClient(List<F1> medecin)
        {
            CallbackRefreshF1ListClient(medecin);
        }

        public event CallbackEventHandler callback;
        public event CallbackEventHandler1 callbackUserjoin;
        public event CallbackEventHandler2 callbackUserLeave;
        public event CallbackEventHandler3 callbackMessageRecu;
        public event CallbackEventHandler4 IsWritingCallbackEvent;
        public event CallbackEventHandler6 InsertF1CallbackEvent;
        public event CallbackEventHandler5 InsertMmebershipCallbackEvent;
        public event CallbackEventHandler7 InsertClientVCallbackEvent;
        public event CallbackEventHandler8 InsertFactureCallbackEvent;
        public event CallbackEventHandler9 InsertTcabCallbackevent;
        public event CallbackEventHandler10 InsertProdfListCallbackevent;
        public event CallbackEventHandler11 InsertFactureVentefListCallbackevent;
        public event CallbackEventHandler12 InsertVerreAssocieCallbackevent;
        public event CallbackEventHandler13 InsertSupplementCallbackevent;
        public event CallbackEventHandler14 InsertIncompatibiliteCallbackevent;
        public event CallbackEventHandler15 InsertMontureListCallbackevent;
        public event CallbackEventHandler16 InsertParamCallbackEvent;
        public event CallbackEventHandler17 InsertLentilleClientListCallbackevent;
        public event CallbackEventHandler18 InsertDepenseCallbackEvent;
        public event CallbackEventHandler19 InsertMotifDepenseCallbackEvent;
        public event CallbackEventHandler20 InsertFournCallbackEvent;
        public event CallbackEventHandler21 InsertProdfCallbackEvent;
        public event CallbackEventHandler22 InsertProduitCallbackEvent;
        public event CallbackEventHandler23 InsertRecoufCallbackEvent;
        public event CallbackEventHandler24 InsertReceptCallbackEvent;
        public event CallbackEventHandler25 InsertdepaiefCallbackEvent;
        public event CallbackEventHandler26 InsertamCallbackEvent;
        public event CallbackEventHandler27 InsertF1ListCallbackEvent;
        public event CallbackEventHandler30 InsertTarifVerreCallbackevent;
        public event CallbackEventHandler31 InsertVerreCallbackevent;
        public event CallbackEventHandler33 InsertCommandeCallbackevent;
        public event CallbackEventHandler34 InsertMontureCallbackevent;
        public event CallbackEventHandler35 InsertLentilleClientCallbackevent;
        public event CallbackEventHandler36 InsertExamenOptométriqueCallbackevent;
        public event CallbackEventHandler37 InsertExamenBinoculaireCallbackevent;
        public event CallbackEventHandler38 InsertAdaptationLentilleCallbackevent;
        public event CallbackEventHandler39 InsertAnamneseCallbackevent;
        public event CallbackEventHandler40 InsertRendezVouCallbackevent;
        public event CallbackEventHandler41 InsertAppointmentCallbackevent;
        public event CallbackEventHandler42 InsertResourceCallbackevent;
        public event CallbackEventHandler43 InsertPaintCallbackevent;
        public event CallbackEventHandler29 InsertLentilleCallbackevent;
        public event CallbackEventHandler32 InsertDepaiemCallbackevent;
        public event CallbackEventHandler47 InsertReceiveFileCallbackevent;
        public event CallbackEventHandler48 InsertReceiveWhisperCallbackevent;
        public event CallbackEventHandler50 InsertReceptProdfCallbackevent;
        public event CallbackEventHandler51 InsertDicomCallbackevent;
        public event CallbackEventHandler52 InsertStatuCallbackevent;
        public event CallbackEventHandler53 InsertMotifTableCallbackevent;
        public event CallbackEventHandler54 InsertDepeiementMultipleFournisseurCallbackevent;
        public event CallbackEventHandler55 InsertMontureSupplementCallbackevent;
        public event CallbackEventHandler56 InsertCatSuppCallbackevent;
        
        public event CallbackEventHandler57 InsertMarqueCallbackevent;

        public event CallbackEventHandler58 InsertFamilleProduitCallbackevent;
        public event CallbackEventHandler49 InsertDepeiementMultipleCallbackevent;
        private void CallbackRefreshLentilleClientList(List<SVC.LentilleClient> clients)
        {
            if (InsertLentilleClientListCallbackevent != null)
            {
                InsertLentilleClientListCallbackevent(this, new CallbackEventInsertListLentilleClient(clients));
            }
        }
        private void CallbackRefreshF1ListClient(List<SVC.F1> clients)
        {
            if (InsertF1ListCallbackEvent != null)
            {
                InsertF1ListCallbackEvent(this, new CallbackEventInsertF1List(clients));
            }
        }
        private void CallbackRefreshMontureList(List<SVC.Monture> clients)
        {
            if (InsertMontureListCallbackevent != null)
            {
                InsertMontureListCallbackevent(this, new CallbackEventInsertListMonture(clients));
            }
        }
        private void CallbackRefreshCatSupp(List<SVC.CatSupp> clients)
        {
            if (InsertCatSuppCallbackevent != null)
            {
                InsertCatSuppCallbackevent(this, new CallbackEventInsertCatSupp(clients));
            }
        }
        private void CallbackRefreshMontureSupplement(SVC.MontureSupplement clients, int oper)
        {
            if (InsertMontureSupplementCallbackevent != null)
            {
                InsertMontureSupplementCallbackevent(this, new CallbackEventInsertMontureSupplement(clients, oper));
            }
        }
        private void CallbackRefreshSupplement(SVC.Supplement clients, int oper)
        {
            if (InsertSupplementCallbackevent != null)
            {
                InsertSupplementCallbackevent(this, new CallbackEventInsertSupplement(clients, oper));
            }
        }
        private void CallbackRefreshIncompatibilite(SVC.Incompatibilite clients, int oper)
        {
            if (InsertIncompatibiliteCallbackevent != null)
            {
                InsertIncompatibiliteCallbackevent(this, new CallbackEventInsertIncompatibilite(clients, oper));
            }
        }
        private void CallbackRefreshVerreAssocie(SVC.VerreAssocie clients, int oper)
        {
            if (InsertVerreAssocieCallbackevent != null)
            {
                InsertVerreAssocieCallbackevent(this, new CallbackEventInsertVerreAssocie(clients, oper));
            }
        }
        private void CallbackRefreshDepeiementMultipleFournisseur(List<SVC.DepeiementMultipleFournisseur> clients)
        {
            if (InsertDepeiementMultipleFournisseurCallbackevent != null)
            {
                InsertDepeiementMultipleFournisseurCallbackevent(this, new CallbackEventInsertDepeiementMultipleFournisseur(clients));
            }
        }
     
        private void CallbackRefreshStatu(List<SVC.Statu> clients)
        {
            if (InsertStatuCallbackevent != null)
            {
                InsertStatuCallbackevent(this, new CallbackEventInsertStatu(clients));
            }
        }
        private void CallbackRefreshMotifTable(List<SVC.MotifTable> clients)
        {
            if (InsertMotifTableCallbackevent != null)
            {
                InsertMotifTableCallbackevent(this, new CallbackEventInsertMotifTable(clients));
            }
        }
        private void CallbackRefreshPaint(SVC.Paint clients, int oper)
        {
            if (InsertPaintCallbackevent != null)
            {
                InsertPaintCallbackevent(this, new CallbackEventInsertPaint(clients, oper));
            }
        }
        private void CallbackRefreshAppointment(SVC.Appointment clients, int oper)
        {
            if (InsertAppointmentCallbackevent != null)
            {
                InsertAppointmentCallbackevent(this, new CallbackEventInsertAppointment(clients, oper));
            }
        }
        private void CallbackRefreshResource(SVC.Resource clients, int oper)
        {
            if (InsertResourceCallbackevent != null)
            {
                InsertResourceCallbackevent(this, new CallbackEventInsertResource(clients, oper));
            }
        }
        private void CallbackRefreshRendezVou(SVC.RendezVou clients, int oper)
        {
            if (InsertRendezVouCallbackevent != null)
            {
                InsertRendezVouCallbackevent(this, new CallbackEventInsertRendezVou(clients, oper));
            }
        }
        private void CallbackRefreshAdaptationLentille(SVC.AdaptationLentille clients, int oper)
        {
            if (InsertAdaptationLentilleCallbackevent != null)
            {
                InsertAdaptationLentilleCallbackevent(this, new CallbackEventInsertAdaptationLentille(clients, oper));
            }
        }
        private void CallbackRefreshAnamnese(SVC.Anamnese clients, int oper)
        {
            if (InsertAnamneseCallbackevent != null)
            {
                InsertAnamneseCallbackevent(this, new CallbackEventInsertAnamnese(clients, oper));
            }
        }
        private void CallbackRefreshExamenBinoculaire(SVC.ExamenBinoculaire clients, int oper)
        {
            if (InsertExamenBinoculaireCallbackevent != null)
            {
                InsertExamenBinoculaireCallbackevent(this, new CallbackEventInsertExamenBinoculaire(clients, oper));
            }
        }
        private void CallbackRefreshExamenOptométrique(SVC.examenopto clients, int oper)
        {
            if (InsertExamenOptométriqueCallbackevent != null)
            {
                InsertExamenOptométriqueCallbackevent(this, new CallbackEventInsertExamenOptométrique(clients, oper));
            }
        }
        private void CallbackRefreshDicomClient(List<DicomFichier> clients)
        {
            if (InsertDicomCallbackevent != null)
            {
                InsertDicomCallbackevent(this, new CallbackEventInsertListImage(clients));
            }
        }
        private void CallbackRefreshLentilleClient(SVC.LentilleClient clients, int oper)
        {
            if (InsertLentilleClientCallbackevent != null)
            {
                InsertLentilleClientCallbackevent(this, new CallbackEventInsertLentilleClient(clients, oper));
            }
        }
        private void CallbackRefreshMonture(SVC.Monture clients, int oper)
        {
            if (InsertMontureCallbackevent != null)
            {
                InsertMontureCallbackevent(this, new CallbackEventInsertMonture(clients, oper));
            }
        }
        private void CallbackRefreshCommande(SVC.Commande clients, int oper)
        {
            if (InsertCommandeCallbackevent != null)
            {
                InsertCommandeCallbackevent(this, new CallbackEventInsertCommande(clients, oper));
            }
        }
        private void CallbackRefreshVerre(Verre clients,int oper)
        {
            if (InsertVerreCallbackevent != null)
            {
                InsertVerreCallbackevent(this, new CallbackEventInsertVerre(clients,oper));
            }
        }
        private void CallbackRefreshLentille(Lentille clients, int oper)
        {
            if (InsertLentilleCallbackevent != null)
            {
                InsertLentilleCallbackevent(this, new CallbackEventInsertLentille(clients, oper));
            }
        }
        private void CallbackRefreshMarque(List<Marque> clients)
        {
            if (InsertMarqueCallbackevent != null)
            {
                InsertMarqueCallbackevent(this, new CallbackEventInsertMarque(clients));
            }
        }
        private void CallbackRefreshListfacturevente(List<Facture> clients)
        {
            if (InsertFactureVentefListCallbackevent != null)
            {
                InsertFactureVentefListCallbackevent(this, new CallbackEventInsertListfacturevente(clients));
            }
        }
        private void CallbackRefreshListProdf(List<Prodf> clients,int oper)
        {
            if (InsertProdfListCallbackevent != null)
            {
                InsertProdfListCallbackevent(this, new CallbackEventInsertListProdf(clients,oper));
            }
        }
        private void CallbackRefreshTcab(List<Tcab> clients)
        {
            if (InsertTcabCallbackevent != null)
            {
                InsertTcabCallbackevent(this, new CallbackEventInsertTcab(clients));
            }
        }
        private void CallbackRefreshTarifVerre(List<TarifVerre> clients)
        {
            if (InsertTarifVerreCallbackevent != null)
            {
                InsertTarifVerreCallbackevent(this, new CallbackEventInsertTarifVerre(clients));
            }
        }
        private void CallbackRefreshDepeiementMultiple(List<DepeiementMultiple> clients)
        {
            if (InsertDepeiementMultipleCallbackevent != null)
            {
                InsertDepeiementMultipleCallbackevent(this, new CallbackEventInsertDepeiementMultiple(clients));
            }
        }
        private void CallbackRefreshFamilleProduit(List<FamilleProduit> medecin)
        {
            if (InsertFamilleProduitCallbackevent != null)
            {
                InsertFamilleProduitCallbackevent(this, new CallbackEventInsertFamilleProduit(medecin));
            }
        }
        private void CallbackRefreshProduit(Produit listMembershipOptic, int oper)
        {

            if (InsertProduitCallbackEvent != null)
            {
                InsertProduitCallbackEvent(this, new CallbackEventInsertProduit(listMembershipOptic, oper));
            }
        }
        private void CallbackRefreshFacture(Facture listMembershipOptic, int oper)
        {

            if (InsertFactureCallbackEvent != null)
            {
                InsertFactureCallbackEvent(this, new CallbackEventInsertFacture(listMembershipOptic, oper));
            }
        }
        private void CallbackRefreshReceiveWhisper(SVC.Message clients, Client receiver)
        {
            if (InsertReceiveWhisperCallbackevent != null)
            {
                InsertReceiveWhisperCallbackevent(this, new CallbackEventReceiveWhisper(clients, receiver));
            }
        }
        private void CallbackRefreshReceiveFile(FileMessage clients,Client receiver)
        {
            if (InsertReceiveFileCallbackevent!=null)
            {
                InsertReceiveFileCallbackevent(this, new CallbackEventReceiveFile(clients,receiver));
            }
        }
        private void fireCallbackEvent(List<Client> clients)
        {
            if (callback != null)
            {
                callback(this, new CallbackEvent(clients));
            }
        }
        private void UserJoined(Client client)
        {
            if (callbackUserjoin != null)
            {
                callbackUserjoin(this, new CallbackEventJoin(client));
            }
        }
        private void UserLeaved(Client client)
        {
            if (callbackUserLeave != null)
            {
                callbackUserLeave(this, new CallbackEventUserLeave(client));
            }
        }
       
             private void UserWrite(Client client)
        {
            if (IsWritingCallbackEvent != null)
            {
                IsWritingCallbackEvent(this, new CallbackEventWriting(client));
            }
        }
        private void ReceivedMessage(Message msg)
        {
            if (callbackMessageRecu != null)
            {
                callbackMessageRecu(this, new CallbackEventMessageRecu(msg));
            }

        }

        private void CallbackInsertMembershipOptic(List<MembershipOptic> listMembershipOptic)
        {
            if (InsertMmebershipCallbackEvent != null)
            {
                InsertMmebershipCallbackEvent(this, new CallbackEventInsertMembershipOptic(listMembershipOptic));
            }

        }

        private void CallbackInsertClientV(SVC.ClientV listMembershipOptic,int oper)
        {
            if (InsertClientVCallbackEvent != null)
            {
                InsertClientVCallbackEvent(this, new CallbackEventInsertClientV(listMembershipOptic,oper));
            }

        }
        private void CallbackInsertF1(SVC.F1 listMembershipOptic, int oper)
        {
            if (InsertF1CallbackEvent != null)
            {
                InsertF1CallbackEvent(this, new CallbackEventInsertF1(listMembershipOptic, oper));
            }

        }

        private void CallbackParam(SVC.Param listMembershipOptic)
        {

            if (InsertParamCallbackEvent != null)
            {
                InsertParamCallbackEvent(this, new CallbackEventInsertParam(listMembershipOptic));
            }
        }
       
        private void CallbackRefreshDepense(Depense listMembershipOptic,int oper)
        {

            if (InsertDepenseCallbackEvent != null)
            {
                InsertDepenseCallbackEvent(this, new CallbackEventInsertDepense(listMembershipOptic,oper));
            }
        }


            private void CallbackRefreshMotifDepense(List<MotifDepense> listMembershipOptic)
        {

            if (InsertMotifDepenseCallbackEvent != null)
            {
                InsertMotifDepenseCallbackEvent(this, new CallbackEventInsertMotifDepense(listMembershipOptic));
            }
        }

        private void CallbackRefreshFourn(List<Fourn> listMembershipOptic)
        {

            if (InsertFournCallbackEvent != null)
            {
                InsertFournCallbackEvent(this, new CallbackEventInsertFourn(listMembershipOptic));
            }
        }

        private void CallbackRefreshProdf(Prodf listMembershipOptic,int oper)
        {

            if (InsertProdfCallbackEvent != null)
            {
                InsertProdfCallbackEvent(this, new CallbackEventInsertProdf(listMembershipOptic,oper));
            }
        }
    
        private void CallbackRefreshRecouf(Recouf listMembershipOptic,int oper)
        {

            if (InsertRecoufCallbackEvent != null)
            {
                InsertRecoufCallbackEvent(this, new CallbackEventInsertRecouf(listMembershipOptic,oper));
            }
        }
        private void CallbackRefreshRecept(List<Recept> listMembershipOptic)
        {

            if (InsertReceptCallbackEvent != null)
            {
                InsertReceptCallbackEvent(this, new CallbackEventInsertRecept(listMembershipOptic));
            }
        }
        private void CallbackRefreshdepaief(depaief listMembershipOptic,int code)
        {

            if (InsertdepaiefCallbackEvent != null)
            {
                InsertdepaiefCallbackEvent(this, new CallbackEventInsertdepaief(listMembershipOptic,code));
            }
        }
        private void CallbackRefresham(List<am> listMembershipOptic)
        {

            if (InsertamCallbackEvent != null)
            {
                InsertamCallbackEvent(this, new CallbackEventInsertam(listMembershipOptic));
            }
        }

      
      
       

      private void CallbackRefreshDepeiment(Depeiment listMembershipOptic,int oper)
        {

            if (InsertDepaiemCallbackevent != null)
            {
                InsertDepaiemCallbackevent(this, new CallbackEventInsertDepeiment(listMembershipOptic,oper));
            }
        }

     
      
     
      

      
        /**************************************************************************************************/
        public IAsyncResult BeginIsWritingCallback(Client client, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceive(Message msg, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();

        }

        public IAsyncResult BeginReceiverFile(FileMessage fileMsg, Client receiver, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginReceiveWhisper(Message msg, Client receiver, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRefreshClients(List<Client> clients, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRefreshClientsChat(List<Client> clients, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginUserJoin(Client client, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginUserLeave(Client client, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndIsWritingCallback(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndReceive(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndReceiverFile(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndReceiveWhisper(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshClients(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshClientsChat(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndUserJoin(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public void EndUserLeave(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

      

      

     

       

        public IAsyncResult BeginRefreshMembership(List<MembershipOptic> MembershipOptic, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshMembership(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        

        public void EndRefreshMedecin(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshClientV(List<SVC.ClientV> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshClientV(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public void EndRefreshRendezVous(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      


        public void EndRefreshSpécialité(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public void EndRefreshMotifVisite(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
        

        public void EndRefreshTypeCas(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

   

        public void EndRefreshCasTraitement(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        


        public void EndRefreshVisite(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public void EndRefreshConstante(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public void EndRefreshDicom(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshParametre(SVC.Param medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshParametre(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public void EndRefreshSalleDattente(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

     

        public IAsyncResult BeginRefreshDepense(List<Depense> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshDepense(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
        public IAsyncResult BeginRefreshMotifDepense(List<MotifDepense> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshMotifDepense(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
        public IAsyncResult BeginRefreshFourn(List<Fourn> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshFourn(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      


        public IAsyncResult BeginRefreshProduit(List<Produit> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshProduit(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        
        public IAsyncResult BeginRefreshProdf(List<Prodf> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshProdf(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
        public IAsyncResult BeginRefreshRecouf(List<Recouf> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshRecouf(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshRecept(List<Recept> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshRecept(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshdepaief(List<depaief> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshdepaief(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefresham(List<am> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefresham(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

    

    

        public void EndRefreshEcranAcceuil(IAsyncResult result)
        {
            throw new NotImplementedException();
        }


       

        public void EndRefreshFamilleAliment(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public void EndRefreshAliment(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public void EndRefreshBesoinCalorique(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

    

        public void EndRefreshRepa(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshDepeiment(List<Depeiment> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshDepeiment(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

       

        public void EndRefreshDci(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

       

        public void EndRefreshCatalogue(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      
      

        public void EndRefreshActe(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

  

        public void EndRefreshProduitOrdonnance(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      
       

        public void EndRefreshOrdonnanceClientV(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

  
       

        public void EndRefreshEnteteOrdonnace(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
      

        public void EndRefreshEtatDent(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

       

        public void EndRefreshBouche(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
       
        public void EndRefreshQuestionnaire(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

      

        public void EndRefreshDiagnostic(IAsyncResult result)
        {
            throw new NotImplementedException();
        }



     

        public void EndRefreshRéponseGuidé(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

    
      

        public void EndRefreshTypeDeProthése(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

     

       

        public void EndRefreshSousTypeProthèse(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

   
        

        public void EndRefreshLaboratoire(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

     

     

        public void EndRefreshLaboProthèseCommande(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshDepeimentMultiple(List<DepeiementMultiple> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshDepeimentMultiple(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
       

        public void EndRefreshautosurveillance(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

     

       

        public void EndRefreshautosurveillanceDetail(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRefreshClientV(SVC.ClientV medecin, int ClientV, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshDepense(Depense medecin, int oper, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

       
     

        public IAsyncResult BeginRefreshProdf(Prodf medecin, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void RefreshProdfRecept(List<Prodf> medecin)
        {
             CallbackRefreshReceptProdf(medecin);
           // throw new NotImplementedException();
        }
        private void CallbackRefreshReceptProdf(List<Prodf> clients)
        {
            if (InsertReceptProdfCallbackevent != null)
            {
                InsertReceptProdfCallbackevent(this, new CallbackEventInsertProdfRecept(clients));
            }
        }


        public IAsyncResult BeginRefreshProdfRecept(List<Prodf> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshProdfRecept(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRefreshDepeiment(Depeiment medecin, int oper, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

       

      

       

        public void EndRefreshArretDetravails(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshProduit(Produit medecin, int operf, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

     

        public IAsyncResult BeginRefreshFamilleProduit(List<FamilleProduit> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshFamilleProduit(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshF1(F1 medecin, int oper, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshF1(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshFacture(Facture medecin, int operf, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshFacture(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshTCab(List<Tcab> medeicn, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshTCab(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshProdflist(List<Prodf> medecin, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshProdflist(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        

        public IAsyncResult BeginRefreshFactureListe(List<Facture> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshFactureListe(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshMarque(List<Marque> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshMarque(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshTarifVerre(List<TarifVerre> medeicn, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshTarifVerre(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshVerre(List<Verre> medeicn, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshVerre(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

     

        public IAsyncResult BeginRefreshLentille(List<Lentille> medeicn, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshLentille(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRefreshLentille(Lentille medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public IAsyncResult BeginRefreshVerre(Verre medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

     

        public IAsyncResult BeginRefreshCommande(SVC.Commande medecin, int oper, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshCommande(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshRecouf(Recouf medecin, int oper, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        

        public IAsyncResult BeginRefreshMonture(Monture medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshMonture(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        

        public IAsyncResult BeginRefreshLentilleClient(LentilleClient medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshLentilleClient(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        

        public IAsyncResult BeginRefreshDicom(List<DicomFichier> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshExamensOptométriques(examenopto medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshExamensOptométriques(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        

        public IAsyncResult BeginRefreshExamenBinoculaire(ExamenBinoculaire medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshExamenBinoculaire(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      
        public IAsyncResult BeginRefreshAdaptationLentille(AdaptationLentille medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshAdaptationLentille(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       


        public IAsyncResult BeginRefreshAnamnese(Anamnese medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshAnamnese(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      
        public IAsyncResult BeginRefreshRendezVou(RendezVou medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshRendezVou(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        

        public IAsyncResult BeginRefreshAppointment(Appointment medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshAppointment(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
        public IAsyncResult BeginRefreshResource(Resource medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshResource(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

     
        public IAsyncResult BeginRefreshPaint(Paint medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshPaint(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshStatu(List<Statu> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshStatu(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        
        public IAsyncResult BeginRefreshMotifTable(List<MotifTable> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshMotifTable(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshDepeiementMultipleFournisseur(List<DepeiementMultipleFournisseur> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshDepeiementMultipleFournisseur(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        

        public IAsyncResult BeginRefreshdepaief(depaief medecin, int oper, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

     

        public IAsyncResult BeginRefreshIncompatibilite(Incompatibilite medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshIncompatibilite(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       

        public IAsyncResult BeginRefreshVerreAssocie(VerreAssocie medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshVerreAssocie(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
        public IAsyncResult BeginRefreshSupplement(Supplement medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshSupplement(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshMontureSupplement(MontureSupplement medeicn, int operF, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshMontureSupplement(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

        
        public IAsyncResult BeginRefreshCatSupp(List<CatSupp> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshCatSupp(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      

        public IAsyncResult BeginRefreshMontureList(List<Monture> medeicn, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshMontureList(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

       
       
        public IAsyncResult BeginRefreshLentilleClientList(List<LentilleClient> medeicn, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshLentilleClientList(IAsyncResult result)
        {
            throw new NotImplementedException();
        }

      
        public IAsyncResult BeginRefreshF1ListClient(List<F1> medecin, AsyncCallback callback, object asyncState)
        {
            throw new NotImplementedException();
        }

        public void EndRefreshF1ListClient(IAsyncResult result)
        {
            throw new NotImplementedException();
        }
    }


    public class CallbackEvent : EventArgs
    {
        private readonly List<Client> clientss;
        public CallbackEvent(List<Client> clients)
        {
            this.clientss = clients;
        }
        public List<Client> clients
        {
            get { return clientss; }
        }
    }

    public class CallbackEventJoin : EventArgs
    {
        private readonly Client clientss;
        public CallbackEventJoin(Client clientsjoin)
        {
            this.clientss = clientsjoin;
        }
        public Client clientsj
        {
            get { return clientss; }
        }
    }
    public class CallbackEventUserLeave : EventArgs
    {
        private readonly Client clientsss;
        public CallbackEventUserLeave(Client clientsjoin)
        {
            this.clientsss = clientsjoin;
        }
        public Client clientleav
        {
            get { return clientsss; }
        }
    }
    public class CallbackEventMessageRecu : EventArgs
    {
        private readonly Message Messagesss;
        public CallbackEventMessageRecu(Message clientsjoin)
        {
            this.Messagesss = clientsjoin;
        }
        public Message clientleav
        {
            get { return Messagesss; }
        }
    }

    public class CallbackEventWriting : EventArgs
    {
        private readonly Client clientsssEcrit;
        public CallbackEventWriting(Client clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public Client clientleav
        {
            get { return clientsssEcrit; }
        }
    }


    public class CallbackEventInsertMembershipOptic : EventArgs
    {
        private readonly List<MembershipOptic> clientsssEcrit;
        public CallbackEventInsertMembershipOptic(List<MembershipOptic> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<MembershipOptic> clientleav
        {
            get { return clientsssEcrit; }
        }
    }


    public class CallbackEventInsertF1 : EventArgs
    {
        private readonly SVC.F1 clientsssEcrit;
        private readonly int opersssEcrit;
        public CallbackEventInsertF1(SVC.F1 clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.opersssEcrit = oper;
        }
        public SVC.F1 clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return opersssEcrit; }
        }
    }
    public class CallbackEventInsertFacture : EventArgs
    {
        private readonly SVC.Facture clientsssEcrit;
        private readonly int opersssEcrit;
        public CallbackEventInsertFacture(SVC.Facture clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.opersssEcrit = oper;
        }
        public SVC.Facture clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return opersssEcrit; }
        }
    }
    public class CallbackEventInsertClientV : EventArgs
    {
        private readonly SVC.ClientV clientsssEcrit;
        private readonly int opersssEcrit;
        public CallbackEventInsertClientV(SVC.ClientV clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.opersssEcrit = oper;
        }
        public SVC.ClientV clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return opersssEcrit; }
        }
    }
  
   
  
 
   

  
   

  
    public class CallbackEventInsertParam : EventArgs
    {
        private readonly SVC.Param clientsssEcrit;
        public CallbackEventInsertParam(SVC.Param clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public SVC.Param clientleav
        {
            get { return clientsssEcrit; }
        }
    }

   
    public class CallbackEventInsertDepense : EventArgs
    {
        private readonly Depense clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertDepense(Depense clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Depense clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertMotifDepense : EventArgs
    {
        private readonly List<MotifDepense> clientsssEcrit;
        public CallbackEventInsertMotifDepense(List<MotifDepense> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<MotifDepense> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertF1List : EventArgs
    {
        private readonly List<F1> clientsssEcrit;
        public CallbackEventInsertF1List(List<F1> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<F1> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertFourn : EventArgs
    {
        private readonly List<Fourn> clientsssEcrit;
        public CallbackEventInsertFourn(List<Fourn> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<Fourn> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertProdfRecept : EventArgs
    {
        private readonly List<Prodf> clientsssEcrit;
        public CallbackEventInsertProdfRecept(List<Prodf> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<Prodf> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertProdf : EventArgs
    {
        private readonly Prodf clientsssEcrit;
        private readonly int opersssEcrit;
        public CallbackEventInsertProdf(Prodf clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.opersssEcrit = oper;
        }
        public Prodf clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return opersssEcrit; }
        }
    }
    public class CallbackEventInsertProduit : EventArgs
    {
        private readonly Produit clientsssEcrit;
        private readonly int opersssEcrit;
        public CallbackEventInsertProduit(Produit clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.opersssEcrit = oper;
        }
        public Produit clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return opersssEcrit; }
        }
    }
    public class CallbackEventInsertCommande : EventArgs
    {
        private readonly SVC.Commande clientsssEcrit;
        private readonly int opersssEcrit;
        public CallbackEventInsertCommande(SVC.Commande clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.opersssEcrit = oper;
        }
        public SVC.Commande clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return opersssEcrit; }
        }
    }
    public class CallbackEventInsertRecouf : EventArgs
    {
        private readonly Recouf clientsssEcrit;
        private readonly int opersssEcrit;
        public CallbackEventInsertRecouf(Recouf clientsjoin,int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.opersssEcrit = oper;
        }
        public Recouf clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return opersssEcrit; }
        }
    }
    public class CallbackEventInsertRecept : EventArgs
    {
        private readonly List<Recept> clientsssEcrit;
        public CallbackEventInsertRecept(List<Recept> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<Recept> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertdepaief : EventArgs
    {
        private readonly depaief clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertdepaief(depaief clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public depaief clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertam : EventArgs
    {
        private readonly List<am> clientsssEcrit;
        public CallbackEventInsertam(List<am> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<am> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    



    public class CallbackEventInsertDepeiment : EventArgs
    {
        private readonly Depeiment clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertDepeiment(Depeiment clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Depeiment clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
  
  

   
 

  
  


   


    public class CallbackEventReceiveFile : EventArgs
    {
        private readonly FileMessage clientsssEcrit;
        private readonly SVC.Client receiver;
        public CallbackEventReceiveFile(FileMessage clientsjoin, SVC.Client receiverClient)
        {
            this.clientsssEcrit = clientsjoin;
            this.receiver = receiverClient;
        }
        public FileMessage clientleav
        {
            get { return clientsssEcrit; }
        }
        public Client clientrec
        {
            get { return receiver; }
        }
    }
    public class CallbackEventReceiveWhisper : EventArgs
    {
        private readonly Message MSG;
        private readonly SVC.Client receiver;


        public CallbackEventReceiveWhisper(Message clientsjoin, SVC.Client receiverClient)
        {
            this.MSG = clientsjoin;
            this.receiver = receiverClient;
        }
        public Message clientleav
        {
            get { return MSG; }
        }
        public Client clientrec
        {
            get { return receiver; }
        }
    }
  
  
  
  
  
  
   
  
   
    public class CallbackEventInsertFamilleProduit : EventArgs
    {
        private readonly List<FamilleProduit> clientsssEcrit;
        public CallbackEventInsertFamilleProduit(List<FamilleProduit> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<FamilleProduit> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertCatSupp : EventArgs
    {
        private readonly List<CatSupp> clientsssEcrit;
        public CallbackEventInsertCatSupp(List<CatSupp> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<CatSupp> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertMotifTable : EventArgs
    {
        private readonly List<MotifTable> clientsssEcrit;
        public CallbackEventInsertMotifTable(List<MotifTable> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<MotifTable> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertStatu : EventArgs
    {
        private readonly List<Statu> clientsssEcrit;
        public CallbackEventInsertStatu(List<Statu> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<Statu> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertMarque : EventArgs
    {
        private readonly List<Marque> clientsssEcrit;
        public CallbackEventInsertMarque(List<Marque> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<Marque> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertDepeiementMultiple : EventArgs
    {
        private readonly List<DepeiementMultiple> clientsssEcrit;
        public CallbackEventInsertDepeiementMultiple(List<DepeiementMultiple> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<DepeiementMultiple> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertDepeiementMultipleFournisseur : EventArgs
    {
        private readonly List<DepeiementMultipleFournisseur> clientsssEcrit;
        public CallbackEventInsertDepeiementMultipleFournisseur(List<DepeiementMultipleFournisseur> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<DepeiementMultipleFournisseur> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertTcab : EventArgs
    {
        private readonly List<Tcab> clientsssEcrit;
        public CallbackEventInsertTcab(List<Tcab> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<Tcab> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
    public class CallbackEventInsertTarifVerre : EventArgs
    {
        private readonly List<TarifVerre> clientsssEcrit;
        public CallbackEventInsertTarifVerre(List<TarifVerre> clientsjoin)
        {
            this.clientsssEcrit = clientsjoin;
        }
        public List<TarifVerre> clientleav
        {
            get { return clientsssEcrit; }
        }
    }
  
   
    public class CallbackEventInsertListImage : EventArgs
    {
        private readonly List<DicomFichier> clientsssEcrit;
        
        public CallbackEventInsertListImage(List<DicomFichier> clientsjoinr)
        {
            this.clientsssEcrit = clientsjoinr;
           
        }
        public List<DicomFichier> clientleav
        {
            get { return clientsssEcrit; }
        }
       
    }
    public class CallbackEventInsertListfacturevente : EventArgs
    {
        private readonly List<Facture> clientsssEcrit;

        public CallbackEventInsertListfacturevente(List<Facture> clientsjoinr)
        {
            this.clientsssEcrit = clientsjoinr;

        }
        public List<Facture> clientleav
        {
            get { return clientsssEcrit; }
        }

    }
    public class CallbackEventInsertListMonture : EventArgs
    {
        private readonly List<Monture> clientsssEcrit;

        public CallbackEventInsertListMonture(List<Monture> clientsjoinr)
        {
            this.clientsssEcrit = clientsjoinr;

        } 
        public List<Monture> clientleav
        {
            get { return clientsssEcrit; }
        }

    }
    public class CallbackEventInsertListLentilleClient : EventArgs
    {
        private readonly List<LentilleClient> clientsssEcrit;

        public CallbackEventInsertListLentilleClient(List<LentilleClient> clientsjoinr)
        {
            this.clientsssEcrit = clientsjoinr;

        }
        public List<LentilleClient> clientleav
        {
            get { return clientsssEcrit; }
        }

    }
    public class CallbackEventInsertLentille : EventArgs
    {
        private readonly Lentille clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertLentille(Lentille clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Lentille clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertVerre : EventArgs
    {
        private readonly Verre clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertVerre(Verre clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Verre clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertIncompatibilite : EventArgs
    {
        private readonly Incompatibilite clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertIncompatibilite(Incompatibilite clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Incompatibilite clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertSupplement : EventArgs
    {
        private readonly Supplement clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertSupplement(Supplement clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Supplement clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertVerreAssocie : EventArgs
    {
        private readonly VerreAssocie clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertVerreAssocie(VerreAssocie clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public VerreAssocie clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertListProdf : EventArgs
    {
        private readonly List<Prodf> clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertListProdf(List<Prodf> clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public List<Prodf> clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertExamenOptométrique : EventArgs
    {
        private readonly examenopto clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertExamenOptométrique(examenopto clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public examenopto clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertExamenBinoculaire : EventArgs
    {
        private readonly ExamenBinoculaire clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertExamenBinoculaire(ExamenBinoculaire clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public ExamenBinoculaire clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertAdaptationLentille : EventArgs
    {
        private readonly AdaptationLentille clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertAdaptationLentille(AdaptationLentille clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public AdaptationLentille clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertAnamnese : EventArgs
    {
        private readonly Anamnese clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertAnamnese(Anamnese clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Anamnese clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertResource : EventArgs
    {
        private readonly Resource clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertResource(Resource clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Resource clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertPaint : EventArgs
    {
        private readonly Paint clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertPaint(Paint clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Paint clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertAppointment : EventArgs
    {
        private readonly Appointment clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertAppointment(Appointment clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Appointment clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertRendezVou : EventArgs
    {
        private readonly RendezVou clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertRendezVou(RendezVou clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public RendezVou clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertMontureSupplement : EventArgs
    {
        private readonly MontureSupplement clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertMontureSupplement(MontureSupplement clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public MontureSupplement clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertMonture : EventArgs
    {
        private readonly Monture clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertMonture(Monture clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public Monture clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    public class CallbackEventInsertLentilleClient : EventArgs
    {
        private readonly LentilleClient clientsssEcrit;
        private readonly int operssEcrit;
        public CallbackEventInsertLentilleClient(LentilleClient clientsjoin, int oper)
        {
            this.clientsssEcrit = clientsjoin;
            this.operssEcrit = oper;
        }
        public LentilleClient clientleav
        {
            get { return clientsssEcrit; }
        }
        public int operleav
        {
            get { return operssEcrit; }
        }
    }
    #endregion
}

