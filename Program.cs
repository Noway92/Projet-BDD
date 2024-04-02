using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO.Ports;
using System.Linq;
using System.Net.Sockets;
using System.Runtime.Remoting.Messaging;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Google.Protobuf.Collections;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.X509;
using Org.BouncyCastle.Math.Field;

namespace Projet_MDD_roméo_noé
{
    internal class Program
    {
        static void Main(string[] args)
        {
            
            MySqlConnection maConnexion = null;
            try
            {
                string connectionString = "SERVER=localhost;PORT=3306;DATABASE=Projet_SQL;UID=root;PASSWORD=Jesaisplus0.;";
                maConnexion = new MySqlConnection(connectionString);
                maConnexion.Open();
            }
            catch (MySqlException e)
            {
                Console.WriteLine("Erreur de connexion : " + e.Message);
                return;
            }
            string type = "";
            do
            {
                Console.WriteLine("Etes vous un client ou un employé ou le patron ?");
                type = Console.ReadLine();
                if(type=="employé")
                {
                    Console.WriteLine("Donnez votre mot de passe : ");
                    string mdp_employé = Console.ReadLine();
                    if (mdp_employé != "employé1234")
                    {
                        Console.WriteLine("Vous vous êtes trompé, nous vous redirigeons au début");
                        type = "";
                    }
                    Console.Clear();
                }
                if (type == "patron")
                {
                    Console.WriteLine("Donnez votre mot de passe de patron : ");
                    string mdp_patron = Console.ReadLine();
                    if (mdp_patron != "patron1234")
                    {
                        Console.WriteLine("Vous vous êtes trompé, nous vous redirigeons au début");
                        type = "";
                    }
                    Console.Clear();
                }

            } while (type != "client" && type != "employé" && type!="patron");
            Console.Clear();
            switch(type)
            {
                // PARTIE CLIENT
                case "client":
                    Création_compte_client(maConnexion);
                    break;
                //PARTIE EMPLOYER
                case "employé":
                    string stop = "";
                    do
                    {
                        int envie = -1;
                        do
                        {
                            try
                            {
                                Console.WriteLine("Voulez vous gérer les commandes : (1) \nRéapprovisionner les magasins en seuil d'alerte (2) : ");
                                envie = Convert.ToInt32(Console.ReadLine());
                            }
                            catch (Exception e)
                            {
                                Console.WriteLine(e.Message);
                            }

                        } while (envie != 1 && envie != 2);
                        Console.Clear();
                        switch (envie)
                        {
                            case 1:
                                analyse_commande(maConnexion);
                                break;
                            case 2:
                                string nom_produit = null;
                                string nom_element = null;
                                string nom = "";
                                Console.WriteLine("Voici la liste des produits standarts : Gros Merci, amoureux, Exotique,Maman et Vive la mariée");
                                Console.WriteLine("Voici la liste des élements individuels : Gerbera, Ginger, Glaïeul, Marguerite et Rose rouge");
                                do
                                {
                                    Console.WriteLine("Choisissez l'élément que vous voulez approvisionner : ");
                                    nom = Console.ReadLine();
                                    if(nom == "amoureux")
                                    {
                                        nom = "L’amoureux";
                                    }
                                    if(nom == "Exotique")
                                    {
                                        nom = "L’Exotique";
                                    }
                                } while (nom != "Gros Merci" && nom != "L’amoureux" && nom != "L’Exotique" && nom != "Maman" && nom != "Vive la mariée" && nom != "Gerbera" && nom != "Ginger" && nom != "Glaïeul" && nom != "Marguerite" && nom != "Rose rouge");
                                if(nom== "Gros Merci" || nom== "L’amoureux" || nom == "L’Exotique" || nom== "Maman" || nom== "Vive la mariée")
                                {
                                    nom_produit = nom;
                                }
                                else
                                {
                                    nom_element = nom;
                                }
                                MySqlCommand command1 = maConnexion.CreateCommand();
                                command1.CommandText = "SELECT adresse_magasin FROM MAGASIN;";
                                MySqlDataReader reader1 = command1.ExecuteReader();
                                string adresse_magasin = "";
                                Console.WriteLine("Voici la liste de tous les magasins que vous pouvez approvisionner :\n");
                                int compteur_magasin = 1;
                                while (reader1.Read())
                                {
                                    Console.WriteLine("Magasin "+compteur_magasin+" : "+reader1.GetString(0));
                                    compteur_magasin++;
                                }
                                reader1.Close();

                                int compteur2 = -1;
                                do
                                {
                                    try
                                    {
                                        Console.WriteLine("Quel magasin voulez vous choisir ? (écrivez son numéro)");
                                        compteur2 = Convert.ToInt32(Console.ReadLine());
                                    }
                                    catch (Exception e)
                                    {
                                        Console.WriteLine(e.Message);
                                    }
                                } while (compteur2 <= 0 || compteur2 > 5);
                                switch (compteur2)
                                {
                                    case (1):
                                        adresse_magasin = "12 rue des Lilas 75019 Paris France";
                                        break;
                                    case (2):
                                        adresse_magasin = "5 avenue de la République 94290 Villeneuve-le-Roi France";
                                        break;
                                    case (3):
                                        adresse_magasin = "25 rue des Ormes 13012 Marseille France";
                                        break;
                                    case (4):
                                        adresse_magasin = "8 rue du Général Leclerc 95130 Franconville France";
                                        break;
                                    case (5):
                                        adresse_magasin = "4 rue du Commerce 91080 Courcouronnes France";
                                        break;

                                }
                                approvisionnement(maConnexion, nom_element, nom_produit, adresse_magasin);
                                break;
                        }

                        Console.WriteLine("Voulez vous vraiment quitter l'interface employé ? (dites stop)");
                        stop = Console.ReadLine();
                        Console.Clear();
                    } while (stop != "stop");

                    
                    
                    break;
                case "patron":
                    string rep = "";
                    do
                    {
                        statistique(maConnexion);
                        Console.WriteLine("Voulez vous continuer ? (oui/non)");
                        rep = Console.ReadLine();
                        Console.Clear();
                    } while (rep != "non");
                    
                    break;
            }                                    
            Console.WriteLine("fin des opérations");
            maConnexion.Close();
            Console.ReadKey();
        }

        static void Création_compte_client(MySqlConnection connexion)
        {
            
            
            int rep = 0;
            do
            {
                Console.WriteLine("Voulez vous creer un compte (tapez 1)\nVoulez vous vous connecter (tapez 2)");
                try
                {
                    rep = Convert.ToInt32(Console.ReadLine());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (rep != 1 && rep != 2);
            Console.Clear();
            string courriel_client=null;
            string mdp_client;
            switch (rep)
            {
                case 1:
                    Console.WriteLine("Donnez votre adresse email : ");
                    courriel_client = Console.ReadLine();
                    Console.WriteLine("Donnez votre mot de passe : ");
                    mdp_client = Console.ReadLine();
                    Console.WriteLine("Donnez votre prénom : ");
                    string prenom_client = Console.ReadLine();
                    Console.WriteLine("Donnez votre nom : ");
                    string nom_client = Console.ReadLine();

                    int num_tel_client = -1;
                    do
                    {
                        try
                        {
                            Console.WriteLine("Donnez votre numéro de téléphone : ");
                            num_tel_client = Convert.ToInt32(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    } while (num_tel_client < 0 || num_tel_client > 10000000000);

                    Console.WriteLine("Donnez votre adresse : ");
                    string adresse_facturation_client =Console.ReadLine();
                    long carte_credit_client = -1;
                    do
                    {
                        try
                        {
                            Console.WriteLine("Donnez votre carte de crédit : ");
                            carte_credit_client =long.Parse(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    } while (carte_credit_client < 0 || carte_credit_client > 10000000000000000 );

                    string statut_client = "";
                    string historique_commande_client = "";
                    DateTime date_creation_client = DateTime.Now;
                    
                    MySqlCommand command1 = connexion.CreateCommand();
                    
                  
                    bool verif = false ;

                    while (verif == false)
                    {
                        command1.CommandText = " SELECT courriel_client FROM CLIENT ;";
                        MySqlDataReader reader1 = command1.ExecuteReader();
                        verif = true ;
                        while (reader1.Read() && verif == true)// parcourt ligne par ligne
                        {
                            //il ne faut pas les "" si on fait une égalité
                            if (courriel_client == reader1.GetString(0))
                            {
                                verif = false;
                            }

                        }
                        //Au cas où la personne s'est trompée
                        reader1.Close();

                        if(verif==false)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.Clear();
                            // On attend 1 seconde pour pouvoir laisser l'écran rouge
                            DateTime attendre = DateTime.Now;
                            while ((DateTime.Now - attendre).TotalSeconds < 1)
                            {

                            }                                                     
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Clear();
                            Console.WriteLine("L'adresse est déjà utilisée");

                            Console.WriteLine("Donnez une autre adresse email : ");
                            courriel_client = Console.ReadLine();
                            Console.WriteLine("Donnez un autre mot de passe : ");
                            mdp_client = Console.ReadLine();
                        }
                        
                        if(verif==true)
                        {
                            MySqlCommand command3 = connexion.CreateCommand();
                            command3.CommandText = " INSERT INTO CLIENT VALUES(@courriel_client,@nom_client,@prenom_client,@num_tel_client,@mdp_client,@adresse_facturation_client,@carte_credit_client,@statut_client,@historique_commande_client,@date_creation_client);";
                            command3.Parameters.AddWithValue("@courriel_client", courriel_client);
                            command3.Parameters.AddWithValue("@nom_client", nom_client);
                            command3.Parameters.AddWithValue("@prenom_client", prenom_client);
                            command3.Parameters.AddWithValue("@num_tel_client", num_tel_client);
                            command3.Parameters.AddWithValue("@mdp_client", mdp_client);
                            command3.Parameters.AddWithValue("@adresse_facturation_client", adresse_facturation_client);
                            command3.Parameters.AddWithValue("@carte_credit_client", carte_credit_client);
                            command3.Parameters.AddWithValue("@statut_client", statut_client);
                            command3.Parameters.AddWithValue("@historique_commande_client", historique_commande_client);
                            command3.Parameters.AddWithValue("@date_creation_client", date_creation_client);
                            command3.ExecuteNonQuery();
                        }
                        
                    }
                   
                    break;
                case 2:
                    // Tester si cela existe
                    bool test = false;
                    MySqlCommand command2 = connexion.CreateCommand();
                    do
                    {                      


                        Console.WriteLine("Donnez votre adresse email : ");
                        courriel_client =Console.ReadLine();
                        Console.WriteLine("Donnez votre mot de passe : ");
                        mdp_client = Console.ReadLine();
                        // Il faut les "" pour le SELECT
                        command2.CommandText = " SELECT courriel_client,mdp_client FROM CLIENT WHERE courriel_client='" + courriel_client + "'AND mdp_client='" + mdp_client + "';";
                        MySqlDataReader reader2 = command2.ExecuteReader();                        
                        while(reader2.Read())
                        {
                            test = true;
                        }
                        if(test == false)
                        {
                            Console.BackgroundColor = ConsoleColor.Red;
                            Console.Clear();
                            // On attend 1 secondes pour pouvoir laisser l'écran rouge
                            DateTime attendre = DateTime.Now;
                            while ((DateTime.Now - attendre).TotalSeconds < 1)
                            {
                                

                            }
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Clear();
                        }
                        if(test == true)
                        {
                            Console.BackgroundColor = ConsoleColor.Green;
                            Console.Clear();
                            // On attend 1 secondes pour pouvoir laisser l'écran vert
                            DateTime attendre = DateTime.Now;
                            while ((DateTime.Now - attendre).TotalSeconds < 1)
                            {


                            }
                            Console.BackgroundColor = ConsoleColor.Black;
                            Console.Clear();

                        }
                        reader2.Close();
                        
                    
                    } while (test == false);
                    break;

            }
            Console.Clear();
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine("Début de la commande : ");
            Console.ResetColor();
            Commander(connexion, courriel_client);
            // On va update le statut du client
            statut_client(connexion, courriel_client);


        }

        static void statut_client(MySqlConnection connexion,string adresse_client)
        {
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = " SELECT numero_commande FROM COMMANDE WHERE courriel_client ='"+adresse_client+"';";
            MySqlDataReader reader = command.ExecuteReader();
            int nb_commande = 0;
            
            // Utile car si il n'y a pas de commande le nb_est mis à 0
            while(reader.Read())
            {
                nb_commande = reader.FieldCount;
            }
            
            reader.Close();

            MySqlCommand command1 = connexion.CreateCommand();
            command1.CommandText = " SELECT date_creation_client FROM CLIENT WHERE courriel_client ='" + adresse_client + "';";
            MySqlDataReader reader1 = command1.ExecuteReader();
            DateTime creer = new DateTime();
            while (reader1.Read())// parcourt ligne par ligne
            {
                
                creer = reader1.GetDateTime(0);
            }            
            reader1.Close();
            int temps = (DateTime.Now.Month - creer.Month)+ (DateTime.Now.Year-creer.Year)*12;
            if(temps==0)
            {
                temps = 1;
            }
            MySqlCommand command2 = connexion.CreateCommand();
            if ((nb_commande/(temps))>5)
            {
                command2.CommandText = " UPDATE CLIENT SET statut_client='OR' WHERE courriel_client ='" + adresse_client + "';";
                command2.ExecuteNonQuery();

            }

            else
            {
                if((nb_commande/(temps))>= 1)
                {
                    command2.CommandText = " UPDATE CLIENT SET statut_client='BRONZE' WHERE courriel_client ='" + adresse_client + "';";
                    command2.ExecuteNonQuery();
                }
                else
                {
                    command2.CommandText = " UPDATE CLIENT SET statut_client='' WHERE courriel_client ='" + adresse_client + "';";
                    command2.ExecuteNonQuery();

                }
            }
        }

        static void Commander(MySqlConnection connexion,string courriel_client)
        {
            double prix_produit = 0;
            //C'est possible que cela ne marche pas car clé étrangère
            string etat_commande = null;
            string nom_produit = null;
            string description_generale_produit = null;
            DateTime date_commande = DateTime.Now;
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = "SELECT MAX(numero_commande) FROM COMMANDE";
            MySqlDataReader reader = command.ExecuteReader();
            long numero_commande = 0;            
            while(reader.Read())
            {
                numero_commande = long.Parse(reader.GetString(0)) + 1;

            }
            reader.Close();
            Console.WriteLine("Pouvez vous écrire votre adresse de livraison dans le format suivant : '12 rue des Lilas 75019 Paris France'");
            string adresse_livraison_commande = Console.ReadLine();
            Console.Clear();
            DateTime date_livraison_commande = new DateTime();
            do
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Pouvez vous entrer la date de livraison désirée dans le format suivant : '31/01/2023'");
                    Console.ResetColor();
                    Console.WriteLine("Cette date doit logiquement être après la date actuelle");
                    string reponse = Console.ReadLine();
                    date_livraison_commande = new DateTime(Convert.ToInt32(reponse.Substring(6, 4)), Convert.ToInt32(reponse.Substring(3, 2)), Convert.ToInt32(reponse.Substring(0, 2)));
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }

            } while (date_livraison_commande < date_commande);
            Console.Clear();
            if ((date_livraison_commande - date_commande).TotalDays < 3)
            {
                Console.WriteLine("Un employé de la boutique vérifiera la disponibilité des items commandés, puis communiquera directement avec vous s'il y a un problème");
            }
            // On regarde si il possède une réduction et on l'appliquera par la suite
            MySqlCommand command2 = connexion.CreateCommand();
            command2.CommandText = " SELECT statut_client FROM CLIENT WHERE courriel_client='"+courriel_client+"';";
            MySqlDataReader reader2 = command2.ExecuteReader();
            double réduction = 1;
            while(reader2.Read())
            {
                if(reader2.GetString(0)=="OR")
                {
                    Console.WriteLine("Grace à votre fidélité OR, vous possédez 15% de réduction sur tous nos produits. (Les nouveaux prix seront directement affichés)");
                    réduction = 0.85;
                }
                if (reader2.GetString(0) == "BRONZE")
                {
                    Console.WriteLine("Grace à votre fidélité BRONZE, vous possédez 5% de réduction sur tous nos produits. (Les nouveaux prix seront directement affichés)");
                    réduction = 0.95;
                }

            }
            reader2.Close();
            
            string rep = "";
            do
            {
                Console.WriteLine("Veux tu prendre un produit standart (taper standart)?\nVeux tu réaliser un produit personnalisé (taper perso)?");
                rep = Console.ReadLine();
            } while (rep != "standart" && rep != "perso");
            Console.Clear();
            switch (rep)
            {
                case "standart":

                    Console.WriteLine("Voici la liste des bouquets disponibles");
                    MySqlCommand command3 = connexion.CreateCommand();
                    command3.CommandText = " SELECT nom_produit,composition_fleurs_produit,prix_produit FROM PRODUITSTANDART;";
                    MySqlDataReader reader3 = command3.ExecuteReader();
                    int compteur = 1;
                    while(reader3.Read())
                    {
                        Console.WriteLine(reader3.GetString(0) + "(" + compteur + ") : " + reader3.GetString(1) + " au modeste prix : " + Convert.ToDouble(reader3.GetString(2)) * réduction);
                        compteur++;
                    }
                    reader3.Close();
                    int bouquet = -1;
                    do
                    {
                        try
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("\nQuel bouquet vous intéresse ? (écrivez son numéro)");
                            Console.ResetColor();
                            bouquet = Convert.ToInt32(Console.ReadLine());
                        }
                        catch(Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }                    
                    } while (bouquet <= 0 || bouquet > 5);
                                     
                    
                    switch (bouquet)
                    {
                        case (1):
                            nom_produit = "Gros Merci";
                            break;
                        case (2):
                            nom_produit = "L’amoureux";
                            break;
                        case (3):
                            nom_produit = "L’Exotique";
                            break;
                        case (4):
                            nom_produit = "Maman";
                            break;
                        case (5):
                            nom_produit = "Vive la mariée";
                            break;
                    }
                    
                    MySqlCommand command14 = connexion.CreateCommand();
                    command14.CommandText = " SELECT prix_produit FROM PRODUITSTANDART WHERE nom_produit= '" + nom_produit + "';";
                    MySqlDataReader reader14 = command14.ExecuteReader();
                    while(reader14.Read())
                    {
                        prix_produit =reader14.GetDouble(0)*réduction;
                    }
                    reader14.Close();
                    
                    if ((date_livraison_commande-date_commande).TotalDays>=3)
                    {
                        //On considère que dans tous les cas la commande sera envoyée
                        etat_commande = "CC";
                    }
                    else
                    {
                        etat_commande = "VINV";
                    }
                    break;

                case "perso":
                    etat_commande = "CPAV";
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Ecrivez ce que vous voulez avec un prix maximal si vous êtes vagues");
                    Console.ResetColor();
                    description_generale_produit = Console.ReadLine();
                    MySqlCommand command1 = connexion.CreateCommand();
                    command1.CommandText = " INSERT INTO PRODUITPERSONNALISE VALUES(@description_generale_produit,@Prix_produit);";
                    command1.Parameters.AddWithValue("@description_generale_produit", description_generale_produit);
                    command1.Parameters.AddWithValue("@Prix_produit", prix_produit);
                    command1.ExecuteNonQuery();                

                    break;
            }
            //Je dois creer un produit personnalisé en fonction de ce qu'il me demandera
            Console.WriteLine("\nVoici la liste des magasins depuis lesquelles vous pouvez vous faire livrer :\n");
            MySqlCommand command4 = connexion.CreateCommand();
            command4.CommandText = " SELECT adresse_magasin FROM MAGASIN;";
            MySqlDataReader reader4 = command4.ExecuteReader();
            int compteur2 = 1;
            while (reader4.Read())
            {
                Console.WriteLine("Magasin " + compteur2 + " : " + reader4.GetString(0));
                compteur2++;
            }
            reader4.Close();
            compteur2 = -1;
            do
            {
                try
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("\nQuel magasin voulez vous choisir ? (écrivez son numéro)");
                    Console.ResetColor();
                    compteur2 = Convert.ToInt32(Console.ReadLine());
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (compteur2 <= 0 || compteur2 > 5);
            string adresse_magasin = "";
            switch (compteur2)
            {
                case (1):
                    adresse_magasin = "12 rue des Lilas 75019 Paris France";
                    break;
                case (2):
                    adresse_magasin = "5 avenue de la République 94290 Villeneuve-le-Roi France";
                    break;
                case (3):
                    adresse_magasin = "25 rue des Ormes 13012 Marseille France";
                    break;
                case (4):
                    adresse_magasin = "8 rue du Général Leclerc 95130 Franconville France";
                    break;
                case (5):
                    adresse_magasin = "4 rue du Commerce 91080 Courcouronnes France";
                    break;

            }
     
            MySqlCommand command5 = connexion.CreateCommand();
            command5.CommandText = " INSERT INTO COMMANDE VALUES(@numero_commande,@date_commande,@adresse_livraison_commande,@date_livraison_commande,@etat_commande,@courriel_client,@nom_produit,@description_generale_produit,@adresse_magasin);";
            command5.Parameters.AddWithValue("@numero_commande", numero_commande);
            command5.Parameters.AddWithValue("@date_commande", date_commande);
            command5.Parameters.AddWithValue("@adresse_livraison_commande", adresse_livraison_commande);
            command5.Parameters.AddWithValue("@date_livraison_commande", date_livraison_commande);
            command5.Parameters.AddWithValue("@etat_commande",etat_commande);
            command5.Parameters.AddWithValue("@courriel_client", courriel_client);
            command5.Parameters.AddWithValue("@nom_produit", nom_produit);
            command5.Parameters.AddWithValue("@description_generale_produit", description_generale_produit);
            command5.Parameters.AddWithValue("@adresse_magasin", adresse_magasin);
            command5.ExecuteNonQuery();
            
            MySqlCommand command6 = connexion.CreateCommand();
            if (nom_produit==null)
            {
                Console.Clear();
                Console.WriteLine("Nous allons analyser votre commande et nous vous ferons parvenir un mail vous donnant le prix et la description, vous aurez 3 jours pour payer");
                if (réduction==0.85)
                {
                    Console.WriteLine("N'oubliez pas que vous possédez une réduction de 15% grace à votre fidélité OR");

                }
                else
                {
                    if(réduction==0.95)
                    {
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("N'oubliez pas que vous possédez une réduction de 5% grace à votre fidélité BRONZE");
                        Console.ResetColor();
                    }
                }
                
                command6.CommandText = " UPDATE CLIENT SET historique_commande_client= CONCAT(historique_commande_client,'perso : ', " + numero_commande + ",', ') WHERE courriel_client = '" + courriel_client + "'; ";
                command6.ExecuteNonQuery();
            }
            else
            {
                command6.CommandText = " UPDATE CLIENT SET historique_commande_client= CONCAT(historique_commande_client,'stand : ', " + numero_commande + ",', ') WHERE courriel_client = '" + courriel_client + "'; ";
                command6.ExecuteNonQuery();
                Console.WriteLine("Vous devez donc payer " + prix_produit + " euros pour cette commande standart");
                MySqlCommand command17 = connexion.CreateCommand();
                command17.CommandText = " SELECT carte_credit_client FROM CLIENT WHERE courriel_client ='" + courriel_client+"';";
                MySqlDataReader reader17 = command17.ExecuteReader();
                long carte = -1; ;
                while (reader17.Read())
                {
                    carte = long.Parse(reader17.GetString(0));
                }
                reader17.Close();
                Console.WriteLine("Voulez vous payer avec votre carte sur votre compte client ? : "+carte+" (oui/non)");
                string reponse = Console.ReadLine();
                if(reponse!="oui")
                {
                    long carte_crédit = -1;
                    do
                    {
                        try
                        {
                            Console.WriteLine("Donnez votre carte de crédit : ");
                            carte_crédit = long.Parse(Console.ReadLine());
                        }
                        catch (Exception e)
                        {
                            Console.WriteLine(e.Message);
                        }

                    } while (carte_crédit < 0 || carte_crédit > 10000000000000000);
                }
                
            }                     
            
            Console.WriteLine("Merci d'avoir commander chez nous !");

        }

        static void statistique(MySqlConnection connexion)
        {
            Console.Write("Voici la liste des choses que vous pouvez voir ");
            int choix = -1;
            {
                try
                {
                    Console.WriteLine("Que voulez vous faire ? (Donner le numéro) \n\nConnaitre le meilleur client (0)\nConnaitre la commande la plus chere (1)\nConnaitre le prix moyen d'un bouquet personnalisé (2)\nConnaitre le produit standart avec le plus de succès (3)\nl'adresse du magasin avec le stock le plus remplis (4)");
                    choix = Convert.ToInt32(Console.ReadLine());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);

                }
            }while(choix<0||choix>4);
            switch(choix)
            {
                case 0:
                    meilleur_client(connexion);
                    break;
                case 1:
                    meilleur_commande(connexion);
                    break;
                case 2:
                    prix_moyen_bouquet(connexion);
                    break;
                case 3:
                    standart_succès(connexion);
                    break;
                case 4:
                    magasin_stock_élevée(connexion);
                    break;
                case 5:

                    break;
                case 6:

                    break;
                case 7:

                    break;
                case 8:
                    
                    break;
            }


        }

        static void meilleur_commande(MySqlConnection connexion)
        {
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = "SELECT MAX(prix_produit) as total_prix FROM (SELECT prix_produit from PRODUITSTANDART P,COMMANDE C where C.nom_produit=P.nom_produit UNION SELECT Prix_produit from PRODUITPERSONNALISE PR,COMMANDE CO where PR.DESCRIPTION_GENERALE_PRODUIT=CO.DESCRIPTION_GENERALE_PRODUIT)as combined_prix;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            double meilleur = 0;
            while (reader.Read())
            {
                meilleur = reader.GetDouble(0);
            }
            reader.Close();
            Console.WriteLine("La commande la plus chere de tous les temps est de : " + meilleur+ " euros");
        }
        static void magasin_stock_élevée(MySqlConnection connexion)

        {
            //Commande avec union et auto-jointure
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = "Select adresse_magasin From MAGASIN";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            List<string> adresse = new List<string>();
            while (reader.Read())
            {
                adresse.Add(reader.GetString(0));
            }
            reader.Close();
            string adresse_final = adresse[0];
            int max = 0;
            for (int i = 0; i < adresse.Count; i++)
            {
                MySqlCommand command1 = connexion.CreateCommand();
                //Requête avec une union
                command1.CommandText = "SELECT SUM(quantite) as total_quantite FROM (SELECT quantite from stocker1 where adresse_magasin='" + adresse[i]+"' UNION SELECT quantite from stocker2 where adresse_magasin ='" + adresse[i]+ "')as quantite_combines;";
                MySqlDataReader reader1;
                reader1 = command1.ExecuteReader();
                while (reader1.Read())
                {
                    if (reader1.GetInt32(0) > max)
                    {
                        max = reader1.GetInt32(0);
                        adresse_final = adresse[i];
                    }
                }
                reader1.Close();


            }

            Console.WriteLine("Le magasin avec le plus de stock est : "+adresse_final);

        }
        static void prix_moyen_bouquet(MySqlConnection connexion)

        {
            MySqlCommand command = connexion.CreateCommand();

            command.CommandText = "SELECT AVG(prix_produit) from produitpersonnalise;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            double prix = 0;
            while (reader.Read())
            {
                prix = reader.GetDouble(0);
            }
            reader.Close();
            Console.WriteLine("Le prix moyen des bouquets personnalisé est " + prix);

        }
        
        static void standart_succès(MySqlConnection connexion)

        {
            
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = "Select nom_produit, Count(*) AS FREQ FROM COMMANDE where nom_produit!= '' Group by(nom_produit) order by FREQ DESC limit 1;";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string nom_produit = "";
            while (reader.Read())
            {
                nom_produit=reader.GetString(0);
            }
            reader.Close();
            

            Console.WriteLine("Le prix bouquet standart ayant le plus de succès est : " + nom_produit);

        }

        static void meilleur_client(MySqlConnection connexion)

        {
            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = "SELECT prenom_client,nom_client from client WHERE historique_commande_client in (SELECT max(historique_commande_client) from client); ";
            MySqlDataReader reader;
            reader = command.ExecuteReader();
            string prenom_client = "";
            string nom_client = "";
            while (reader.Read())
            {
                prenom_client = reader.GetString(0);
                nom_client = reader.GetString(1);
            }
            reader.Close();
            Console.WriteLine("Le meilleur client est " + nom_client+" "+prenom_client);

        }

        static void analyse_commande(MySqlConnection connexion)
        {
            

            MySqlCommand command = connexion.CreateCommand();
            command.CommandText = "SELECT numero_commande FROM COMMANDE where etat_commande!= 'CL';";
            MySqlDataReader reader = command.ExecuteReader();
            List<long> num_commande = new List<long>();
            while (reader.Read())
            {
                num_commande.Add(long.Parse(reader.GetString(0)));              
            }
            reader.Close();
            bool pause = false;
            for(int i=0;i<num_commande.Count && pause==false; i++)
            {
                MySqlCommand command7 = connexion.CreateCommand();
                command7.CommandText = "SELECT adresse_magasin FROM COMMANDE WHERE numero_commande='" + num_commande[i] + "';";
                MySqlDataReader reader7 = command7.ExecuteReader();
                string adresse_magasin = "";
                while (reader7.Read())
                {
                    adresse_magasin = reader7.GetString(0);
                }

                reader7.Close();
                MySqlCommand command1 = connexion.CreateCommand();
                command1.CommandText = "SELECT etat_commande FROM COMMANDE where numero_commande='" +num_commande[i]+"';";
                MySqlDataReader reader1 = command1.ExecuteReader();
                string etat_commande = "";

                while(reader1.Read())
                {
                    etat_commande = reader1.GetString(0);
                }
                reader1.Close();
                
                switch (etat_commande)
                {
                    case "VINV":
                        string nom_produit="";
                        MySqlCommand command5 = connexion.CreateCommand();
                        command5.CommandText = "SELECT nom_produit FROM COMMANDE WHERE numero_commande ='" + num_commande[i] + "';";
                        MySqlDataReader reader5 = command5.ExecuteReader();
                        while(reader5.Read())
                        {
                            nom_produit = reader5.GetString(0);
                        }
                        reader5.Close();
                        MySqlCommand command6 = connexion.CreateCommand();
                        //1 Requête avec auto-jointure 
                        command6.CommandText = "SELECT quantite FROM stocker2 where adresse_magasin='" + adresse_magasin + "' AND nom_produit='" + nom_produit + "';";
                        MySqlDataReader reader6 = command6.ExecuteReader();
                        int quantite = 0;
                        while (reader6.Read())
                        {
                            quantite = reader6.GetInt32(0);
                        }
                        reader6.Close();

                       
                        if (quantite>=1)
                        {
                            MySqlCommand command11 = connexion.CreateCommand();
                            command11.CommandText = " UPDATE COMMANDE SET etat_commande = 'CC' WHERE numero_commande = '" + num_commande[i] + "';";
                            command11.ExecuteNonQuery();

                            quantite--;
                            

                            MySqlCommand command8 = connexion.CreateCommand();
                            command8.CommandText = " UPDATE STOCKER2 SET quantite=" + quantite + " WHERE adresse_magasin='" + adresse_magasin + "' AND nom_produit='" + nom_produit + "';";
                            command8.ExecuteNonQuery();
                            if (quantite<8)
                            {
                                Console.WriteLine("Le magasin situé à l'adresse : " + adresse_magasin + " est en ETAT d'ALERTE");
                                MySqlCommand command9 = connexion.CreateCommand();
                                command9.CommandText = " UPDATE MAGASIN SET seuil_alerte=true WHERE adresse_magasin='" +adresse_magasin+"';";
                                command9.ExecuteNonQuery();
                            }
                            Console.WriteLine("Le produit : " + nom_produit + " est bien disponible dans le magasin situé à l'adresse suivante : " + adresse_magasin);
                        }
                        else
                        {
                            Console.WriteLine("Le produit : " + nom_produit + " n'est plus disponible dans le magasin situé à l'adresse suivante : " + adresse_magasin);
                        }

                        if(quantite<8)
                        {
                            Console.WriteLine("Voulez vous réapprovisionner ce magasin en " + nom_produit + ", il en reste "+quantite+"? (oui/non)");
                            if(Console.ReadLine()=="oui")
                            {
                                approvisionnement(connexion, null, nom_produit, adresse_magasin);
                            }
                        }

                        break;

                    case "CPAV":
                        double prix = 0;
                        MySqlCommand command19 = connexion.CreateCommand();
                        command19.CommandText = "SELECT description_generale_produit FROM COMMANDE where numero_commande ='"+ num_commande[i] +"';";
                        MySqlDataReader reader19 = command19.ExecuteReader();
                        string description = "";
                        while (reader19.Read())
                        {
                            description = reader19.GetString(0);
                        }
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Voici la description que le client à donné : " + description);
                        Console.ResetColor();
                        reader19.Close();
                        string arret = "oui";
                        int prix_produit = 0;
                        while(arret!="non")
                        {
                            string nom_element = "";
                            do
                            {
                                Console.WriteLine("Donner le nom de l'élément que veut le client :\nGerbera, Ginger, Glaïeul, Marguerite ou Rose rouge");
                                nom_element = Console.ReadLine();
                            } while (nom_element != "Gerbera" && nom_element != "Ginger" && nom_element!= "Glaïeul"&& nom_element != "Marguerite" && nom_element != "Rose rouge");
                            
                           
                            int quantite_element = 0;
                            do
                            {
                                try
                                {
                                    Console.WriteLine("Donner la quantité de cette élément");
                                    quantite_element = Convert.ToInt32(Console.ReadLine());
                                }
                                catch(Exception e)
                                {
                                    Console.WriteLine(e.Message);
                                }
                            } while (quantite_element <= 0);

                            MySqlCommand command20 = connexion.CreateCommand();
                            //1 Requête synchronosée
                            command20.CommandText = "SELECT quantite FROM stocker1 Where nom_element IN (SELECT nom_element FROM ELEMENT WHERE nom_element IN(SELECT nom_element FROM CONTIENT WHERE description_generale_produit ='"+description+"')) AND adresse_magasin ='"+adresse_magasin +"' ;";
                            MySqlDataReader reader20 = command20.ExecuteReader();
                            int nombre = 0;
                            while (reader20.Read())
                            {
                                nombre = reader20.GetInt32(0);
                            }
                            reader20.Close();
                            nombre = nombre - quantite_element;
                            if (nombre<0)
                            {
                                Console.WriteLine("Le nombre d'élément : " + nom_element + " n'est pas disponible dans le magasin situé à l'adresse suivante : " + adresse_magasin);
                                if (quantite_element < 20)
                                {
                                    Console.WriteLine("Voulez vous réapprovisionner ce magasin en " + nom_element + ", il en reste " + nombre+quantite_element + "? (oui/non)");
                                    if (Console.ReadLine() == "oui")
                                    {
                                        approvisionnement(connexion, nom_element, null, adresse_magasin);
                                    }
                                }
                            }
                            else
                            {
                                Console.WriteLine("Le nombre d'élément : " + nom_element + " est bien dispoible dans le magasin situé à l'adresse suivante : " + adresse_magasin);
                                MySqlCommand command21 = connexion.CreateCommand();
                                //1 Requête synchronosée
                                command21.CommandText = "SELECT prix_element FROM ELEMENT WHERE nom_element='"+nom_element+"';";
                                MySqlDataReader reader21 = command21.ExecuteReader();
                                double prix_element = 0;
                                while (reader21.Read())
                                {
                                    prix_element = reader21.GetDouble(0);
                                }
                                reader21.Close();

                                prix += quantite_element * prix_element;
                                MySqlCommand command27 = connexion.CreateCommand();
                                command27.CommandText = " UPDATE COMMANDE SET etat_commande = 'CC' WHERE numero_commande = '" + num_commande[i] + "';";
                                command27.ExecuteNonQuery();

                                MySqlCommand command8 = connexion.CreateCommand();
                                command8.CommandText = " UPDATE STOCKER1 SET quantite=" + nombre + " WHERE adresse_magasin='" + adresse_magasin + "' AND nom_element='" + nom_element + "';";
                                command8.ExecuteNonQuery();


                                if (nombre< 8)
                                {
                                    Console.ForegroundColor = ConsoleColor.Red;
                                    Console.WriteLine("Le magasin situé à l'adresse : " + adresse_magasin + " est en ETAT d'ALERTE");
                                    Console.ResetColor();
                                    MySqlCommand command25 = connexion.CreateCommand();
                                    command25.CommandText = " UPDATE MAGASIN SET seuil_alerte=true WHERE adresse_magasin='" + adresse_magasin + "';";
                                    command25.ExecuteNonQuery();
                                    
                                    Console.WriteLine("Voulez vous réapprovisionner ce magasin en " + nom_element + ", il en reste " + nombre + "? (oui/non)");
                                    if (Console.ReadLine() == "oui")
                                    {
                                        approvisionnement(connexion, nom_element, null, adresse_magasin);
                                    }
                                }


                            }

                            

                            Console.WriteLine("Il y a t-il d'autres éléments ? (oui/non)");
                            arret = Console.ReadLine();
                        }
                        Console.WriteLine("Le mail ci dessous a bien été envoyé : \nBonjour, votre commande numéro : " + num_commande[i] + " est bien complète.\nVous devez payer : " + prix + " euros.\nMerci beaucoup pour votre commande et à très vite\n");
                        MySqlCommand command30 = connexion.CreateCommand();
                        command30.CommandText = " UPDATE PRODUITPERSONNALISE SET Prix_produit=" + prix + " WHERE description_generale_produit='" + description + "';";
                        command30.ExecuteNonQuery();

                        break;
                    case "CAL":
                        MySqlCommand command3 = connexion.CreateCommand();
                        command3.CommandText = "SELECT date_livraison_commande FROM COMMANDE WHERE numero_commande='" + num_commande[i] + "';";
                        MySqlDataReader reader3 = command3.ExecuteReader();
                        DateTime date_livraison= new DateTime();
                        while (reader3.Read())
                        {
                            date_livraison = reader3.GetDateTime(0);
                        }
                        reader3.Close();

                        if(date_livraison<DateTime.Now)
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Le colis numéro : " + num_commande[i] + " a bien été livré");
                            Console.ResetColor();
                            MySqlCommand command4 = connexion.CreateCommand();
                            command4.CommandText = " UPDATE COMMANDE SET etat_commande = 'CL' WHERE numero_commande = '" + num_commande[i] + "';";
                            command4.ExecuteNonQuery();
                        }
                        else
                        {
                            Console.ForegroundColor = ConsoleColor.Red;
                            Console.WriteLine("Le colis numéro : " + num_commande[i] + " n'a pas encore été livré");
                            Console.ResetColor();
                        }                        
                        break;
                    case "CC":
                        Console.ForegroundColor = ConsoleColor.Red;
                        Console.WriteLine("Tu peux aller mettre la commande numéro : " + num_commande[i] + " dans le bac des commandes à livrer");
                        Console.ResetColor();
                        MySqlCommand command10 = connexion.CreateCommand();
                        command10.CommandText = " UPDATE COMMANDE SET etat_commande = 'CAL' WHERE numero_commande = '" + num_commande[i] + "';";
                        command10.ExecuteNonQuery();
                        break;
                } 
                Console.WriteLine("Veux-tu arreter de gérer les commandes ? (écris oui ou non)");
                if(Console.ReadLine()=="oui")
                {
                    pause = true;
                }
                Console.Clear();


            }
            


        }
        
        static void approvisionnement(MySqlConnection connexion,string nom_element,string nom_produit,string adresse_magasin)
        {

            int quantite = 0;

            MySqlCommand command6 = connexion.CreateCommand();
            if(nom_produit==null)
            {
                command6.CommandText = "SELECT quantite FROM stocker1 where adresse_magasin='" + adresse_magasin + "' AND nom_element='" + nom_element + "';";
            }
            else
            {
                command6.CommandText = "SELECT quantite FROM stocker2 where adresse_magasin='" + adresse_magasin + "' AND nom_produit='" + nom_produit + "';";
            }           
            
            MySqlDataReader reader6 = command6.ExecuteReader();
            while (reader6.Read())
            {
                quantite = reader6.GetInt32(0);
            }
            reader6.Close();

            int nombre = -1;
            do
            {
                try
                {
                    Console.WriteLine("Combien de produits/éléments voulez vous acheter ?");
                    nombre = Convert.ToInt32(Console.ReadLine());
                }
                catch(Exception e)
                {
                    Console.WriteLine(e.Message);
                }
            } while (nombre <= 0);         
        

            MySqlCommand command = connexion.CreateCommand();
            quantite += nombre;
            if(nom_element==null)
            {
                command.CommandText = " UPDATE STOCKER2 SET quantite = "+quantite+" WHERE adresse_magasin = '" + adresse_magasin + "' AND nom_produit='"+nom_produit+"';";
            }
            else
            {
                command.CommandText = " UPDATE STOCKER1 SET quantite ="+quantite+ " WHERE adresse_magasin = '" + adresse_magasin + "' AND nom_element='" + nom_element + "';";
            }
            command.ExecuteNonQuery();
        }
    }
}
